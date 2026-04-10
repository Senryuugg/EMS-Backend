import pandas as pd
import pickle
import numpy as np
from math import radians, sin, cos, sqrt, atan2
import requests
import time
import logging
import os

logger = logging.getLogger(__name__)

class DistanceCalculator:
    """Handles distance calculations between coordinates using different methods."""
    
    def __init__(self, api_key=None, average_speed=30):
        self.api_key = api_key
        self.average_speed = average_speed  # km/h
        self.use_road_network = api_key is not None and api_key != "your_api_key_here"
    
    def haversine_distance(self, coord1, coord2):
        """Calculate the great-circle distance between two points on Earth in km."""
        R = 6371 
        lat1, lon1 = map(radians, coord1)
        lat2, lon2 = map(radians, coord2)
        dlat = lat2 - lat1
        dlon = lon2 - lon1
        a = sin(dlat / 2) ** 2 + cos(lat1) * cos(lat2) * sin(dlon / 2) ** 2
        c = 2 * atan2(sqrt(a), sqrt(1 - a))
        return R * c
    
    def get_route_info(self, start_coords, end_coords):
        """Get road distance and duration between two points using OpenRouteService API."""
        if not self.use_road_network:
            # Calculate using haversine if no API key
            distance = self.haversine_distance(start_coords, end_coords)
            time_estimate = (distance / self.average_speed) * 60
            return distance, time_estimate, False
            
        base_url = "https://api.openrouteservice.org/v2/directions/driving-car"
        
        # Format coordinates for ORS API (lon,lat format)
        coordinates = [[start_coords[1], start_coords[0]], [end_coords[1], end_coords[0]]]
        
        headers = {
            "Authorization": self.api_key,
            "Content-Type": "application/json; charset=utf-8" 
        }
        
        data = {
            "coordinates": coordinates
        }
        
        try:
            response = requests.post(base_url, headers=headers, json=data, timeout=10)
            if response.status_code == 200:
                data = response.json()
                # Extract distance (in meters) and duration (in seconds)
                distance_km = data['routes'][0]['summary']['distance'] / 1000
                duration_min = data['routes'][0]['summary']['duration'] / 60
                return distance_km, duration_min, True
            else:
                logger.warning(f"API error: {response.status_code}")
                # Fall back to haversine
                distance = self.haversine_distance(start_coords, end_coords)
                time_estimate = (distance / self.average_speed) * 60
                return distance, time_estimate, False
        except Exception as e:
            logger.warning(f"Error getting route information: {e}")
            # Fall back to haversine
            distance = self.haversine_distance(start_coords, end_coords)
            time_estimate = (distance / self.average_speed) * 60
            return distance, time_estimate, False


class HospitalPredictor:
    """Handles hospital prediction based on patient data."""
    
    # Marikina city bounding box
    MARIKINA_BBOX = {
        'lat_min': 14.60,
        'lat_max': 14.68,
        'lon_min': 121.07,
        'lon_max': 121.13
    }
    
    # Valid input values
    VALID_SEVERITIES = ['low', 'medium', 'high']
    VALID_CONDITIONS = [
        'Minor injury', 'Fever', 'Laceration', 
        'Fracture', 'Moderate respiratory distress', 'Abdominal pain',  
        'Heart attack', 'Major trauma', 'Stroke'  
    ]
    
    def __init__(self):
        self.model = None
        self.le_severity = None
        self.le_condition = None
        self.hospitals = None
        self.ems_bases = None
        self.distance_calculator = None
        
    def load_models_and_data(self, api_key=None):
        """Load all required models and data."""
        try:
            # Get the script directory
            script_dir = os.path.dirname(os.path.abspath(__file__))
            
            # Load model and encoders
            model_path = os.path.join(script_dir, 'models', 'hospital_prediction_model.pkl')
            severity_path = os.path.join(script_dir, 'models', 'le_severity.pkl')
            condition_path = os.path.join(script_dir, 'models', 'le_condition.pkl')
            hospital_data_path = os.path.join(script_dir, 'datasets', 'hospital', 'hospital_dataset (cleaned).csv')
            
            with open(model_path, 'rb') as f:
                self.model = pickle.load(f)

            with open(severity_path, 'rb') as f:
                self.le_severity = pickle.load(f)

            with open(condition_path, 'rb') as f:
                self.le_condition = pickle.load(f)

            # Load hospital dataset for distance calculations
            self.hospitals = pd.read_csv(hospital_data_path)
            self.hospitals['location'] = self.hospitals[['Latitude', 'Longtitude']].values.tolist()
            
            # Initialize distance calculator
            self.distance_calculator = DistanceCalculator(api_key=api_key)
            
            # Define EMS bases
            self.ems_bases = [
                {'base_id': 163, 'base_name': '163 Base - Barangay Hall IVC', 'latitude': 14.6270218, 'longitude': 121.0797032},
                {'base_id': 166, 'base_name': '166 Base - CHO Office, Barangay Sto.niño', 'latitude': 14.6399746, 'longitude': 121.0965973},
                {'base_id': 167, 'base_name': '167 Base - Barangay Hall Kalumpang', 'latitude': 14.624179, 'longitude': 121.0933239},
                {'base_id': 164, 'base_name': '164 Base - DRRMO Building, Barangay Fortune', 'latitude': 14.6628689, 'longitude': 121.1214235},
                {'base_id': 165, 'base_name': '165 Base - St. Benedict Barangay Nangka', 'latitude': 14.6737274, 'longitude': 121.108795},
                {'base_id': 169, 'base_name': '169 Base - Pugad Lawin, Barangay Fortune', 'latitude': 14.6584306, 'longitude': 121.1312048}
            ]
            
            logger.info("Models and data loaded successfully")
            return True
        except Exception as e:
            logger.error(f"Error loading models and data: {e}")
            return False
    
    def get_closest_ems_base(self, patient_location):
        """Find the closest EMS base to the patient."""
        if not self.ems_bases:
            raise ValueError("EMS bases not loaded")
            
        ems_base_distances = []
        
        for base in self.ems_bases:
            base_coords = [base['latitude'], base['longitude']]
            
            distance, travel_time, is_road_network = self.distance_calculator.get_route_info(
                base_coords, patient_location
            )
            
            ems_base_distances.append({
                'base_id': base['base_id'],
                'base_name': base['base_name'],
                'coords': base_coords,
                'distance': distance,
                'time': travel_time,
                'is_road_distance': is_road_network
            })
        
        # Select the closest EMS base
        closest_ems_base = min(ems_base_distances, key=lambda x: x['time'])
        return closest_ems_base
    
    def get_hospital_distances(self, patient_location):
        """Calculate distances from patient to all hospitals."""
        if self.hospitals is None:
            raise ValueError("Hospital data not loaded")
            
        hospital_info = []
        
        for idx, hospital in self.hospitals.iterrows():
            hospital_coords = hospital['location']
            hospital_id = hospital['ID']
            
            distance, travel_time, is_road_network = self.distance_calculator.get_route_info(
                patient_location, hospital_coords
            )
            hospital_info.append((hospital_id, distance, travel_time, not is_road_network))
            
            # Small delay to avoid rate limiting
            time.sleep(0.05)
            
        return hospital_info
    
    def predict_hospital(self, latitude, longitude, severity, condition, closest_ems_base, hospital_info):
        """Predict the most appropriate hospital."""
        # Find closest hospital for distance calculation
        closest_hospital = min(hospital_info, key=lambda x: x[1])
        distance_to_hospital_km = closest_hospital[1]

        # Calculate response time components
        time_to_patient = closest_ems_base['time']
        time_to_hospital = closest_hospital[2]
        
        # Fixed time components (in minutes)
        dispatch_time = 2 
        on_scene_time = 10  
        handover_time = 5  

        # Total response time calculation
        response_time_min = dispatch_time + time_to_patient + on_scene_time + time_to_hospital + handover_time

        # Create input DataFrame for model
        new_patient = pd.DataFrame({
            'latitude': [latitude],
            'longitude': [longitude],
            'severity': [severity],
            'condition': [condition],
            'distance_to_hospital_km': [distance_to_hospital_km],
            'response_time_min': [response_time_min]
        })

        # Preprocess new data
        new_patient['severity'] = self.le_severity.transform(new_patient['severity'])
        new_patient['condition'] = self.le_condition.transform(new_patient['condition'])

        # Predict hospital
        predicted_hospital_id = self.model.predict(new_patient)[0]
        
        # Get hospital info
        hospital_name = self.hospitals[self.hospitals['ID'] == predicted_hospital_id]['Name'].iloc[0]
        hospital_level = "Unknown"  # Default value
        if 'Level' in self.hospitals.columns:
            hospital_level = self.hospitals[self.hospitals['ID'] == predicted_hospital_id]['Level'].iloc[0]
            
        # Find predicted hospital in hospital_info
        predicted_hospital_info = next((info for info in hospital_info if info[0] == predicted_hospital_id), None)
        
        result = {
            'hospital_id': predicted_hospital_id,
            'hospital_name': hospital_name,
            'hospital_level': hospital_level,
            'distance': distance_to_hospital_km,
            'time_components': {
                'dispatch_time': dispatch_time,
                'time_to_patient': time_to_patient,
                'on_scene_time': on_scene_time,
                'time_to_hospital': time_to_hospital,
                'handover_time': handover_time,
                'total_time': response_time_min
            },
            'is_fallback_calculation': predicted_hospital_info[3] if predicted_hospital_info else True,
            'ems_base': closest_ems_base
        }
        
        return result
