# BugTrack

## description
This project represents my learning journey in full-stack development,\
combining .NET and React to create a practical and user-friendly application\
showcasing my abilities in building functional and maintainable software.

The server-side is .NET\
The client-side is React (typescript)


## Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- [Node.js](https://nodejs.org/)
- [npm](https://www.npmjs.com/) (Node Package Manager)

### Sqlite for the database with entity framework core.
- cd Bugtrack/API
- dotnet ef database update


# Getting Started
### 1. Clone the repository

### 2. set up the back-end
Navigate to the API project folder
cd Bugtrack/API  
- dotnet restore           - restore project dependencies
- dotnet build             - build the project
- dotnet run               - run the api server

Visit http://localhost:5000 in your browser to see if the API is running.


### 3. set up the client-side
Navigate to the client-side project folder
cd Bugtrack/client-app  
- npm install               - Install client-side dependencies
- npm run dev               - Start the development server\
Visit http://localhost:3000 in your browser to see your React app.
