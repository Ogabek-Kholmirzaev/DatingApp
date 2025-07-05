# DatingApp

DatingApp is a modern, full-stack web application designed to connect people and help them find meaningful relationships. It features a secure authentication system, real-time messaging, photo management, and an intuitive user interface for both regular users and administrators.

## Live Demo

Hosted at: [http://tanishish.somee.com](http://tanishish.somee.com)

---

## Features
- User registration and authentication (JWT)
- Profile management
- Photo upload and approval (admin/moderator)
- Real-time messaging (SignalR)
- Like and match system
- Admin panel for user and photo management
- Responsive UI with Bootstrap and ngx-bootstrap

## Technologies Used
- **Backend:** ASP.NET Core Web API
- **Frontend:** Angular 17
- **Database:** SQL Server (Entity Framework Core)
- **Real-time:** SignalR
- **Authentication:** JWT Bearer Tokens
- **UI:** Bootstrap, ngx-bootstrap, FontAwesome

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js & npm](https://nodejs.org/)
- [Angular CLI](https://angular.io/cli)
- SQL Server (local or cloud)

### Setup Instructions

1. **Clone the repository**
   ```sh
   git clone <your-repo-url>
   cd DatingApp
   ```

2. **Configure the API**
   - Update `API/appsettings.json` with your database connection string and any required API keys (e.g., for Cloudinary).

3. **Run the API**
   ```sh
   cd API
   dotnet restore
   dotnet ef database update
   dotnet run
   ```
   The API will be available at `https://localhost:5001` or `http://localhost:5000` by default.

4. **Run the Angular Client**
   ```sh
   cd client
   npm install
   ng build --configuration production
   # or for development
   ng serve
   ```
   The client will be available at `http://localhost:4200` (if using `ng serve`).

5. **Production Build**
   - The Angular build output is configured to be placed in `API/wwwroot` for seamless integration with the ASP.NET Core backend.

## Deployment
- The app is currently hosted at [http://tanishish.somee.com](http://tanishish.somee.com)
- For deployment, publish the API project and ensure the Angular build files are in the `wwwroot` folder.

## License
This project is for educational purposes. See the repository for more details.