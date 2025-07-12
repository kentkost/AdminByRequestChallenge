Remove-Item -Path "AdminByRequestChallenge.DataContext/Migrations" -Recurse -Force -ErrorAction SilentlyContinue

dotnet ef migrations add InitialAuthDb --context AuthContext --verbose --project AdminByRequestChallenge.DataContext/ --startup-project  AdminByRequestChallenge.API --configuration Release --output-dir Migrations/InitialAuthDb

dotnet ef database update --context AuthContext --verbose --project AdminByRequestChallenge.DataContext --startup-project  AdminByRequestChallenge.API
