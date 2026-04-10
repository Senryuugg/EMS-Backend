# EMS Dispatcher - Deployment Guide

## Production Deployment

This guide covers deploying the EMS Dispatcher system to production environments.

### Prerequisites

- Docker & Docker Compose
- Kubernetes cluster (optional, for large scale)
- MongoDB Atlas or managed MongoDB instance
- Azure Container Registry or Docker Hub account
- CI/CD platform (GitHub Actions, Azure Pipelines)

## Docker Build & Push

### 1. Build Backend Image

```bash
cd EmsDispatch.Backend
docker build -t ems-dispatch-backend:1.0.0 .
docker tag ems-dispatch-backend:1.0.0 your-registry.azurecr.io/ems-dispatch-backend:1.0.0
docker push your-registry.azurecr.io/ems-dispatch-backend:1.0.0
```

### 2. Build ML Service Image

```bash
docker build -t ems-dispatch-ml:1.0.0 -f Dockerfile.ml .
docker tag ems-dispatch-ml:1.0.0 your-registry.azurecr.io/ems-dispatch-ml:1.0.0
docker push your-registry.azurecr.io/ems-dispatch-ml:1.0.0
```

## Kubernetes Deployment

### 1. Create Namespace

```bash
kubectl create namespace ems-dispatch
```

### 2. Backend Deployment

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: ems-backend
  namespace: ems-dispatch
spec:
  replicas: 3
  selector:
    matchLabels:
      app: ems-backend
  template:
    metadata:
      labels:
        app: ems-backend
    spec:
      containers:
      - name: backend
        image: your-registry.azurecr.io/ems-dispatch-backend:1.0.0
        ports:
        - containerPort: 80
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: MongoDB__ConnectionString
          valueFrom:
            secretKeyRef:
              name: mongodb-secret
              key: connectionstring
        - name: Jwt__Secret
          valueFrom:
            secretKeyRef:
              name: jwt-secret
              key: secret
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health
            port: 80
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health
            port: 80
          initialDelaySeconds: 5
          periodSeconds: 5
---
apiVersion: v1
kind: Service
metadata:
  name: ems-backend-service
  namespace: ems-dispatch
spec:
  selector:
    app: ems-backend
  ports:
  - protocol: TCP
    port: 80
    targetPort: 80
  type: LoadBalancer
```

### 3. ML Service Deployment

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: ems-ml-service
  namespace: ems-dispatch
spec:
  replicas: 2
  selector:
    matchLabels:
      app: ems-ml-service
  template:
    metadata:
      labels:
        app: ems-ml-service
    spec:
      containers:
      - name: ml-service
        image: your-registry.azurecr.io/ems-dispatch-ml:1.0.0
        ports:
        - containerPort: 8000
        env:
        - name: PORT
          value: "8000"
        resources:
          requests:
            memory: "512Mi"
            cpu: "500m"
          limits:
            memory: "1Gi"
            cpu: "1000m"
        livenessProbe:
          httpGet:
            path: /health
            port: 8000
          initialDelaySeconds: 30
          periodSeconds: 10
---
apiVersion: v1
kind: Service
metadata:
  name: ems-ml-service
  namespace: ems-dispatch
spec:
  selector:
    app: ems-ml-service
  ports:
  - protocol: TCP
    port: 8000
    targetPort: 8000
  type: ClusterIP
```

## Environment Configuration

### Backend Production Secrets

```bash
kubectl create secret generic mongodb-secret \
  --from-literal=connectionstring='mongodb+srv://user:pass@cluster.mongodb.net/ems_dispatch' \
  -n ems-dispatch

kubectl create secret generic jwt-secret \
  --from-literal=secret='your-jwt-secret-key' \
  -n ems-dispatch
```

## CI/CD Pipeline (GitHub Actions)

```yaml
name: Build and Deploy EMS Dispatcher

on:
  push:
    branches: [main]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Login to Azure Container Registry
        uses: azure/docker-login@v1
        with:
          login-server: ${{ secrets.REGISTRY_LOGIN_SERVER }}
          username: ${{ secrets.REGISTRY_USERNAME }}
          password: ${{ secrets.REGISTRY_PASSWORD }}
      
      - name: Build Backend Image
        run: |
          docker build -t ${{ secrets.REGISTRY_LOGIN_SERVER }}/ems-backend:${{ github.sha }} -f EmsDispatch.Backend/Dockerfile .
          docker push ${{ secrets.REGISTRY_LOGIN_SERVER }}/ems-backend:${{ github.sha }}
      
      - name: Build ML Service Image
        run: |
          docker build -t ${{ secrets.REGISTRY_LOGIN_SERVER }}/ems-ml:${{ github.sha }} -f Dockerfile.ml .
          docker push ${{ secrets.REGISTRY_LOGIN_SERVER }}/ems-ml:${{ github.sha }}
      
      - name: Deploy to Kubernetes
        uses: azure/aks-set-context@v3
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}
          cluster-name: ${{ secrets.CLUSTER_NAME }}
          resource-group: ${{ secrets.RESOURCE_GROUP }}
      
      - name: Update Kubernetes Deployment
        run: |
          kubectl set image deployment/ems-backend \
            backend=${{ secrets.REGISTRY_LOGIN_SERVER }}/ems-backend:${{ github.sha }} \
            -n ems-dispatch
          kubectl set image deployment/ems-ml-service \
            ml-service=${{ secrets.REGISTRY_LOGIN_SERVER }}/ems-ml:${{ github.sha }} \
            -n ems-dispatch
```

## Database Setup

### MongoDB Atlas Setup

1. Create cluster on MongoDB Atlas
2. Configure network access whitelist
3. Create database user
4. Get connection string
5. Set in environment variables

### Backup Strategy

```bash
# Automated daily backup
mongodump --uri="mongodb+srv://user:pass@cluster.mongodb.net/ems_dispatch" \
  --archive=/backups/ems-dispatch-$(date +%Y%m%d).archive

# Restore from backup
mongorestore --uri="mongodb+srv://user:pass@cluster.mongodb.net/ems_dispatch" \
  --archive=/backups/ems-dispatch-20240101.archive
```

## Mobile App Distribution

### iOS (App Store)

1. Create Apple Developer account
2. Create App ID and provisioning profiles
3. Build release IPA: `flutter build ipa --release`
4. Upload via Xcode Organizer or Transporter
5. Submit for review

### Android (Google Play)

1. Create Google Play Developer account
2. Generate signing key: `keytool -genkey -v -keystore ~/key.jks -keyalg RSA -keysize 2048 -validity 10000 -alias upload`
3. Build signed APK: `flutter build apk --release`
4. Upload to Google Play Console
5. Configure staged rollout

## Monitoring & Logging

### Application Insights (Azure)

```csharp
// In Program.cs
builder.Services.AddApplicationInsightsTelemetry();
```

### Logging Configuration

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  }
}
```

## Security Hardening

### API Security

- Enable HTTPS only
- Configure CORS properly
- Implement rate limiting
- Add API authentication
- Validate all inputs
- Sanitize database queries

### Database Security

- Enable encryption at rest
- Use strong passwords
- Enable IP whitelisting
- Regular backups
- Enable audit logging

### Mobile App Security

- Encrypt sensitive data at rest
- Use secure token storage
- Implement certificate pinning
- Obfuscate code
- Regular security audits

## Performance Tuning

### Database Indexes

```javascript
// MongoDB indexes for optimal performance
db.dispatches.createIndex({ status: 1, createdAt: -1 });
db.dispatches.createIndex({ assignedDriverId: 1 });
db.drivers.createIndex({ currentLocation: "2dsphere" });
db.locations.createIndex({ createdAt: 1 }, { expireAfterSeconds: 86400 });
```

### Caching Strategy

- Redis for session cache
- Application-level caching
- CDN for static assets
- Browser caching for mobile app

## Scaling Considerations

### Horizontal Scaling

- Use load balancer for API
- Stateless backend design
- Separate database layer
- Independent ML service scaling

### Database Scaling

- MongoDB replica set for HA
- Sharding for large datasets
- Connection pooling
- Query optimization

## Disaster Recovery

### RTO & RPO Targets

- RTO (Recovery Time Objective): 4 hours
- RPO (Recovery Point Objective): 1 hour

### Backup Location

- Primary: Cloud storage
- Secondary: On-premises
- Frequency: Daily automated

## Rollback Procedure

```bash
# Rollback to previous version
kubectl rollout undo deployment/ems-backend -n ems-dispatch
kubectl rollout undo deployment/ems-ml-service -n ems-dispatch

# Verify rollback
kubectl rollout status deployment/ems-backend -n ems-dispatch
```

## Health Checks

### API Health Endpoint

```
GET /api/health
Response: { status: "healthy", timestamp: "2024-01-15T10:30:00Z" }
```

### ML Service Health

```
GET /health
Response: { status: "ok", models_loaded: true }
```

## Post-Deployment Checklist

- [ ] Verify all pods are running
- [ ] Check database connectivity
- [ ] Test API endpoints
- [ ] Verify ML service predictions
- [ ] Check SignalR connections
- [ ] Monitor error logs
- [ ] Performance baseline established
- [ ] Alerts configured
- [ ] Backup verified
- [ ] Documentation updated

## Troubleshooting

### Pod Crashes

```bash
# Check pod logs
kubectl logs pod-name -n ems-dispatch
kubectl logs pod-name -n ems-dispatch --previous

# Describe pod for events
kubectl describe pod pod-name -n ems-dispatch
```

### Database Connection Issues

- Verify connection string format
- Check IP whitelist
- Verify credentials
- Test network connectivity

### Performance Issues

- Monitor resource usage
- Check database indexes
- Review slow queries
- Analyze API response times

---

For more information, see the main [README.md](./README.md) and [EMS_SETUP.md](./EMS_SETUP.md).
