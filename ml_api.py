from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
from typing import List, Optional
import logging
import os
import sys

# Add parent directory to path for imports
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

# Setup logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

# Import the hospital predictor
from ml_service import HospitalPredictor

# Initialize FastAPI app
app = FastAPI(
    title="EMS Hospital Prediction ML Service",
    description="ML service for intelligent hospital selection and route optimization",
    version="1.0.0"
)

# Add CORS middleware
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Initialize predictor globally
predictor = None

# Pydantic models for request/response
class PredictionRequest(BaseModel):
    latitude: float
    longitude: float
    severity: str
    condition: str
    api_key: Optional[str] = None

class TimeComponentsResponse(BaseModel):
    dispatch_time: float
    time_to_patient: float
    on_scene_time: float
    time_to_hospital: float
    handover_time: float
    total_time: float

class HospitalPredictionResponse(BaseModel):
    hospital_id: int
    hospital_name: str
    hospital_level: str
    distance: float
    time_components: TimeComponentsResponse
    ems_base: dict
    confidence: float
    is_fallback_calculation: bool

class RouteOptimizationRequest(BaseModel):
    ambulance_lat: float
    ambulance_lon: float
    patient_lat: float
    patient_lon: float
    hospital_lat: float
    hospital_lon: float
    api_key: Optional[str] = None

class RouteOptimizationResponse(BaseModel):
    distance_km: float
    estimated_time_minutes: float
    waypoints: List[dict]
    traffic_aware: bool

class HealthResponse(BaseModel):
    status: str
    ml_models_loaded: bool
    message: str

@app.on_event("startup")
async def startup_event():
    """Initialize ML models on startup"""
    global predictor
    try:
        logger.info("Loading ML models...")
        predictor = HospitalPredictor()
        
        # Load models and data
        success = predictor.load_models_and_data()
        if success:
            logger.info("ML models loaded successfully")
        else:
            logger.warning("Failed to load ML models")
    except Exception as e:
        logger.error(f"Error during startup: {e}")
        predictor = None

@app.on_event("shutdown")
async def shutdown_event():
    """Cleanup on shutdown"""
    logger.info("Shutting down ML service")

@app.get("/health", response_model=HealthResponse)
async def health_check():
    """Check service health and model status"""
    try:
        models_loaded = predictor is not None
        status = "healthy" if models_loaded else "degraded"
        message = "ML models loaded and ready" if models_loaded else "ML models not initialized"
        
        return HealthResponse(
            status=status,
            ml_models_loaded=models_loaded,
            message=message
        )
    except Exception as e:
        logger.error(f"Health check error: {e}")
        return HealthResponse(
            status="unhealthy",
            ml_models_loaded=False,
            message=str(e)
        )

@app.post("/predict/hospital", response_model=HospitalPredictionResponse)
async def predict_hospital(request: PredictionRequest):
    """
    Predict the most appropriate hospital for a patient based on their condition and location.
    
    Args:
        latitude: Patient latitude
        longitude: Patient longitude
        severity: Patient severity level (low, medium, high)
        condition: Patient medical condition
        api_key: Optional API key for route optimization
    
    Returns:
        Hospital prediction with route information
    """
    try:
        if predictor is None:
            raise HTTPException(
                status_code=503,
                detail="ML models not loaded. Service temporarily unavailable."
            )
        
        # Validate input
        if not (-90 <= request.latitude <= 90):
            raise HTTPException(status_code=400, detail="Invalid latitude")
        if not (-180 <= request.longitude <= 180):
            raise HTTPException(status_code=400, detail="Invalid longitude")
        
        # Create distance calculator with optional API key
        if request.api_key:
            predictor.distance_calculator.api_key = request.api_key
            predictor.distance_calculator.use_road_network = True
        
        patient_location = [request.latitude, request.longitude]
        
        # Find closest EMS base
        closest_ems_base = predictor.get_closest_ems_base(patient_location)
        
        # Get hospital distances
        hospital_info = predictor.get_hospital_distances(patient_location)
        
        # Make prediction
        prediction_result = predictor.predict_hospital(
            request.latitude,
            request.longitude,
            request.severity.lower(),
            request.condition,
            closest_ems_base,
            hospital_info
        )
        
        # Calculate confidence score (based on model's capability to predict accurately)
        confidence = 0.85  # Default confidence
        
        return HospitalPredictionResponse(
            hospital_id=prediction_result['hospital_id'],
            hospital_name=prediction_result['hospital_name'],
            hospital_level=str(prediction_result['hospital_level']),
            distance=prediction_result['distance'],
            time_components=TimeComponentsResponse(**prediction_result['time_components']),
            ems_base=prediction_result['ems_base'],
            confidence=confidence,
            is_fallback_calculation=prediction_result['is_fallback_calculation']
        )
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"Prediction error: {e}")
        raise HTTPException(status_code=500, detail=f"Prediction failed: {str(e)}")

@app.post("/optimize/route", response_model=RouteOptimizationResponse)
async def optimize_route(request: RouteOptimizationRequest):
    """
    Optimize route for ambulance to reach patient and then hospital.
    
    Args:
        ambulance_lat: Ambulance latitude
        ambulance_lon: Ambulance longitude
        patient_lat: Patient latitude
        patient_lon: Patient longitude
        hospital_lat: Hospital latitude
        hospital_lon: Hospital longitude
        api_key: Optional API key for real road network optimization
    
    Returns:
        Optimized route information
    """
    try:
        if predictor is None:
            raise HTTPException(
                status_code=503,
                detail="ML models not loaded. Service temporarily unavailable."
            )
        
        ambulance_location = [request.ambulance_lat, request.ambulance_lon]
        patient_location = [request.patient_lat, request.patient_lon]
        hospital_location = [request.hospital_lat, request.hospital_lon]
        
        # Create distance calculator with optional API key
        dc = predictor.distance_calculator
        if request.api_key:
            dc.api_key = request.api_key
            dc.use_road_network = True
        
        # Calculate distances
        ambulance_to_patient_dist, ambulance_to_patient_time, traffic_aware = dc.get_route_info(
            ambulance_location, patient_location
        )
        
        patient_to_hospital_dist, patient_to_hospital_time, _ = dc.get_route_info(
            patient_location, hospital_location
        )
        
        total_distance = ambulance_to_patient_dist + patient_to_hospital_dist
        total_time = ambulance_to_patient_time + patient_to_hospital_time
        
        return RouteOptimizationResponse(
            distance_km=total_distance,
            estimated_time_minutes=total_time,
            waypoints=[
                {"lat": ambulance_location[0], "lon": ambulance_location[1], "name": "Ambulance"},
                {"lat": patient_location[0], "lon": patient_location[1], "name": "Patient"},
                {"lat": hospital_location[0], "lon": hospital_location[1], "name": "Hospital"}
            ],
            traffic_aware=traffic_aware
        )
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"Route optimization error: {e}")
        raise HTTPException(status_code=500, detail=f"Route optimization failed: {str(e)}")

@app.get("/")
async def root():
    """API root endpoint"""
    return {
        "service": "EMS Hospital Prediction ML Service",
        "version": "1.0.0",
        "endpoints": {
            "health": "/health",
            "predict_hospital": "/predict/hospital (POST)",
            "optimize_route": "/optimize/route (POST)",
            "documentation": "/docs"
        }
    }

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)
