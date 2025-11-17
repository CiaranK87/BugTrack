# BugTrack Application Deployment Guide

This guide will help you deploy the BugTrack application with Vercel (frontend) and Azure (backend API).

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Database Setup](#database-setup)
3. [Email Service Setup](#email-service-setup)
4. [Backend Deployment (Azure)](#backend-deployment-azure)
5. [Frontend Deployment (Vercel)](#frontend-deployment-vercel)
6. [Post-Deployment Configuration](#post-deployment-configuration)
7. [Security Considerations](#security-considerations)

## Prerequisites

1. **Azure Account**: Create an Azure account at [https://azure.microsoft.com](https://azure.microsoft.com)
2. **Vercel Account**: Create a Vercel account at [https://vercel.com](https://vercel.com)
3. **Domain Name**: Have a custom domain ready for your portfolio
4. **Git Repository**: Ensure your code is pushed to a Git repository (GitHub, GitLab, etc.)

## Database Setup

### Option 1: Azure Database for PostgreSQL (Recommended)

1. Log in to the [Azure Portal](https://portal.azure.com)
2. Click "Create a resource" → "Databases" → "Azure Database for PostgreSQL"
3. Select "Flexible Server" and click "Create"
4. Configure the server:
   - **Server name**: Choose a unique name
   - **Region**: Choose a region close to your users
   - **PostgreSQL version**: Use the latest stable version
   - **Workload type**: Development (can be changed later)
   - **Compute + storage**: Start with Burstable B1ms (1 vCore, 2GB RAM)
   - **Storage size**: Start with 32GB
5. Configure **Networking**:
   - Select "Public access" for now (can be restricted later)
   - Add your current IP to the firewall rules
6. Configure **Authentication**:
   - Select "Password authentication"
   - Set a strong admin password
7. Review and create the server
8. Once deployed, go to the database and create a new database named `bugtrack`

### Option 2: Neon (Free PostgreSQL Alternative)

1. Sign up at [https://neon.tech](https://neon.tech)
2. Create a new project
3. Copy the connection string (it will look like: `postgresql://username:password@ep-xxx.us-east-2.aws.neon.tech/dbname?sslmode=require`)

### Option 3: Supabase (Free PostgreSQL with Additional Features)

1. Sign up at [https://supabase.com](https://supabase.com)
2. Create a new project
3. Go to Settings → Database to get the connection string

## Email Service Setup

### Using Resend (Recommended)

1. Sign up at [https://resend.com](https://resend.com)
2. Verify your domain:
   - Go to Domains → Add Domain
   - Add your portfolio domain (e.g., yourportfolio.com)
   - Follow the DNS verification steps
3. Once verified, go to API Keys → Create API Key
4. Copy the API key for later use

## Backend Deployment (Azure)

### 1. Prepare Environment Variables

Create a copy of the `.env` file in the `API` directory and update it with your production values:

```bash
# Database Configuration
DB_CONNECTION_STRING=postgresql://username:password@your-db-host:5432/bugtrack

# JWT Configuration
JWT_SECRET_KEY=your-super-secure-jwt-secret-key-here
JWT_ISSUER=https://your-azure-app-name.azurewebsites.net
JWT_AUDIENCE=https://your-portfolio-domain.com

# Email Configuration
RESEND_API_KEY=re_your_resend_api_key_here
FROM_EMAIL=noreply@your-portfolio-domain.com
ADMIN_EMAIL=your-email@example.com

# CORS Configuration
ALLOWED_ORIGINS=https://your-portfolio-domain.com

# Production Flag
ASPNETCORE_ENVIRONMENT=Production
```

### 2. Deploy to Azure App Service

#### Option A: Using Azure Portal (Recommended for Beginners)

1. Log in to the [Azure Portal](https://portal.azure.com)
2. Click "Create a resource" → "Web" → "Web App"
3. Configure the web app:
   - **Name**: Choose a unique name (e.g., `bugtrack-api-yourname`)
   - **Runtime stack**: .NET 8
   - **Operating system**: Linux
   - **Region**: Same region as your database
   - **App Service Plan**: Standard S1 (or B1 for basic usage)
4. Click "Review + create" → "Create"
5. Once deployed, go to the web app:
   - Go to "Configuration" → "Application settings"
   - Add all the environment variables from step 1
6. Deploy your code:
   - Go to "Deployment Center"
   - Connect to your Git repository
   - Select the repository and branch
   - Azure will automatically build and deploy your API

#### Option B: Using Azure CLI (Advanced)

```bash
# Install Azure CLI if not already installed
# Log in to Azure
az login

# Create a resource group
az group create --name BugTrackResources --location "East US"

# Create an App Service plan
az appservice plan create --name BugTrackPlan --resource-group BugTrackResources --sku S1 --is-linux

# Create the web app
az webapp create --name bugtrack-api-yourname --resource-group BugTrackResources --plan BugTrackPlan --runtime "DOTNETCORE|8.0"

# Set environment variables
az webapp config appsettings set --name bugtrack-api-yourname --resource-group BugTrackResources --settings DB_CONNECTION_STRING="your_connection_string"
az webapp config appsettings set --name bugtrack-api-yourname --resource-group BugTrackResources --settings JWT_SECRET_KEY="your_jwt_secret"
# Add all other environment variables...

# Configure deployment from GitHub
az webapp deployment source config --name bugtrack-api-yourname --resource-group BugTrackResources --repo-url "https://github.com/yourusername/BugTrack.git" --branch "main" --manual-integration
```

### 3. Run Database Migrations

Once your API is deployed, you need to run the database migrations:

1. SSH into your Azure App Service:
   ```bash
   az webapp ssh --name bugtrack-api-yourname --resource-group BugTrackResources
   ```
2. Navigate to the app directory and run migrations:
   ```bash
   cd /home/site/wwwroot
   dotnet ef database update
   ```

## Frontend Deployment (Vercel)

### 1. Update Vercel Configuration

Edit the `client-app/vercel.json` file to include your actual API URL:

```json
{
  "version": 2,
  "builds": [
    {
      "src": "package.json",
      "use": "@vercel/static-build",
      "config": {
        "distDir": "dist"
      }
    }
  ],
  "env": {
    "VITE_API_URL": "https://bugtrack-api-yourname.azurewebsites.net/api"
  },
  "rewrites": [
    {
      "source": "/(.*)",
      "destination": "/index.html"
    }
  ]
}
```

### 2. Deploy to Vercel

#### Using Vercel Dashboard (Recommended)

1. Log in to [Vercel Dashboard](https://vercel.com/dashboard)
2. Click "Add New..." → "Project"
3. Import your Git repository
4. Configure the project:
   - **Framework Preset**: Vite
   - **Root Directory**: `client-app`
   - **Build Command**: `npm run build`
   - **Output Directory**: `dist`
5. Add environment variables:
   - `VITE_API_URL`: `https://bugtrack-api-yourname.azurewebsites.net/api`
6. Click "Deploy"

#### Using Vercel CLI (Advanced)

```bash
# Install Vercel CLI
npm i -g vercel

# Login to Vercel
vercel login

# Deploy from the client-app directory
cd client-app
vercel --prod
```

### 3. Configure Custom Domain

1. In the Vercel Dashboard, go to your project settings
2. Go to "Domains" → "Add" your custom domain
3. Follow the DNS configuration steps
4. Once verified, your app will be available at your custom domain

## Post-Deployment Configuration

### 1. Test the Application

1. Visit your frontend URL and test:
   - Registration flow (should show contact form)
   - Login functionality
   - Project and ticket creation
   - Comment functionality

2. Test the API directly:
   - Visit `https://bugtrack-api-yourname.azurewebsites.net/api/health`
   - Should return a healthy status

### 2. Create an Admin Account

1. Access the admin registration endpoint directly:
   ```
   POST https://bugtrack-api-yourname.azurewebsites.net/api/account/admin/register
   ```
2. Include the required fields in the request body

### 3. Configure Monitoring

1. Set up Azure Monitor for your API:
   - Go to your App Service in Azure Portal
   - Enable Application Insights
   - Set up alerts for errors and high response times

2. Set up Vercel Analytics for your frontend:
   - Go to your project settings in Vercel
   - Enable Analytics

## Security Considerations

### 1. Database Security

- Restrict database access to only your Azure App Service IP
- Use SSL connections (enabled by default in Azure PostgreSQL)
- Regularly update your database password

### 2. API Security

- Ensure all sensitive data is in environment variables
- Regularly rotate your JWT secret key
- Monitor logs for unusual activity

### 3. Frontend Security

- Ensure all API calls are made over HTTPS
- Implement proper error handling
- Consider adding a Content Security Policy (CSP)

### 4. Email Security

- Use domain authentication (SPF, DKIM, DMARC)
- Monitor email sending limits
- Implement rate limiting on the contact endpoint

## Troubleshooting

### Common Issues

1. **Database Connection Errors**:
   - Check connection string format
   - Verify firewall rules
   - Ensure database is running

2. **CORS Errors**:
   - Verify ALLOWED_ORIGINS environment variable
   - Check that your frontend URL is included

3. **JWT Token Issues**:
   - Verify JWT_SECRET_KEY is set
   - Check JWT_ISSUER and JWT_AUDIENCE values

4. **Email Not Sending**:
   - Verify RESEND_API_KEY
   - Check domain verification status in Resend
   - Verify FROM_EMAIL address

### Getting Help

- Check Azure App Service logs
- Check Vercel deployment logs
- Review browser console for frontend errors
- Use network tab to inspect API requests

## Maintenance

### Regular Tasks

1. **Database Backups**: Ensure automated backups are configured
2. **Dependency Updates**: Regularly update NuGet and npm packages
3. **Security Patches**: Apply security updates promptly
4. **Log Review**: Regularly review application logs

### Scaling Considerations

1. **Database Scaling**: Monitor database performance and scale as needed
2. **App Service Scaling**: Consider scaling out if traffic increases
3. **CDN**: Consider using Azure CDN for static assets

## Conclusion

Your BugTrack application is now deployed and ready for use! The application includes:

- Secure user authentication with JWT tokens
- Role-based access control
- Email notifications for access requests
- PostgreSQL database for production
- Security headers and CORS configuration
- Health monitoring endpoints

For any issues or questions, refer to the troubleshooting section or check the application logs.