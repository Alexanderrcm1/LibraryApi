
# This script is used to test the endpoints
param(
    [string]$environment = "Development",
    [string]$launchProfile = "https-Development",
    [string]$connectionStringKey = "BooksDb",
    [bool]$dropDatabase = $false,
    [bool]$createDatabase = $false
)

# Get the project name
$projectName = Get-ChildItem -Recurse -Filter "*.csproj" | Select-Object -First 1 | ForEach-Object { $_.Directory.Name } # Projectname can also be set manually

# Get the base URL of the project
$launchSettings = Get-Content -LiteralPath ".\$projectName\Properties\launchSettings.json" | ConvertFrom-Json
$baseUrl = ($launchSettings.profiles.$launchProfile.applicationUrl -split ";")[0] # Can also set manually -> $baseUrl = "https://localhost:7253"

#Install module SqlServer
if (-not (Get-Module -ErrorAction Ignore -ListAvailable SqlServer)) {
    Write-Verbose "Installing SqlServer module for the current user..."
    Install-Module -Scope CurrentUser SqlServer -ErrorAction Stop
}
Import-Module SqlServer

# Set the environment variable
$env:ASPNETCORE_ENVIRONMENT = $environment



# Read the connection string from appsettings.Development.json
$appSettings = Get-Content ".\$projectName\appsettings.$environment.json" | ConvertFrom-Json
$connectionString = $appSettings.ConnectionStrings.$connectionStringKey
Write-Host "Database Connection String: $connectionString" -ForegroundColor Blue


# Get the database name from the connection string
if ($connectionString -match "Database=(?<dbName>[^;]+)")
{
    $databaseName = $matches['dbName']
    Write-Host "Database Name: $databaseName" -ForegroundColor Blue
}else{
    Write-Host "Database Name not found in connection string" -ForegroundColor Red
    exit
}


# Check if the database exists
$conStringDbExcluded = $connectionString -replace "Database=[^;]+;", ""
$queryDbExists = Invoke-Sqlcmd -ConnectionString $conStringDbExcluded -Query "Select name FROM sys.databases WHERE name='$databaseName'"
if($queryDbExists){
    if($dropDatabase -or (Read-Host "Do you want to drop the database? (y/n)").ToLower() -eq "y") { 

        # Drop the database
        Invoke-Sqlcmd -ConnectionString $connectionString -Query  "USE master;ALTER DATABASE $databaseName SET SINGLE_USER WITH ROLLBACK IMMEDIATE;DROP DATABASE $databaseName;"
        Write-Host "Database $databaseName dropped." -ForegroundColor Green
    }
}

# Create the database from the model
if(Select-String -LiteralPath ".\$projectName\Program.cs" -Pattern "EnsureCreated()"){
    Write-Host "The project uses EnsureCreated() to create the database from the model." -ForegroundColor Yellow
} else {
    if($createDatabase -or (Read-Host "Should dotnet ef migrate and update the database? (y/n)").ToLower() -eq "y") { 

        dotnet ef migrations add "UpdateModelFromScript_$(Get-Date -Format "yyyyMMdd_HHmmss")" --project ".\$projectName\$projectName.csproj"
        dotnet ef database update --project ".\$projectName\$projectName.csproj"
    }
}

# Run the application
if((Read-Host "Start the server from Visual studio? (y/n)").ToLower() -ne "y") { 
    Start-Process -FilePath "dotnet" -ArgumentList "run --launch-profile $launchProfile --project .\$projectName\$projectName.csproj" -WindowStyle Normal    
    Write-Host "Wait for the server to start..." -ForegroundColor Yellow 
}

# Continue with the rest of the script
Read-Host "Press Enter to continue when the server is started..."


### ------------------------------
### ------------------------------

Write-Host "Running Script"

$apiAuthorsEndpoint = "$baseurl/api/Authors"
$apiBooksEndpoint = "$baseurl/api/Books"
$apiBorrowersEndpoint = "$baseurl/api/Borrowers"
$apiLoansEndpoint = "$baseurl/api/Loans"

# --- Posting author ---
$author = @{ 
    FirstName = "David";
    LastName = "Goggins";
 } | ConvertTo-Json
$response = Invoke-RestMethod -Uri $apiAuthorsEndpoint -Method Post -Body $author -ContentType 'application/json'
$response | Format-Table

# --- Posting author ---
$author = @{ 
    FirstName = "Arnold";
    LastName = "Schwarzenegger"; 
} | ConvertTo-Json
$response = Invoke-RestMethod -Uri $apiAuthorsEndpoint -Method Post -Body $author -ContentType 'application/json'
$response | Format-Table

# --- Posting author ---
$author = @{
    FirstName = "James";
    LastName = "Clear";
} | ConvertTo-Json
$response = Invoke-RestMethod -Uri $apiAuthorsEndpoint -Method Post -Body $author -ContentType 'application/json'
$response | Format-Table

# --- Posting author ---
$author = @{
    FirstName = "J.K";
    LastName = "Rowlings";
} | ConvertTo-Json
$response = Invoke-RestMethod -Uri $apiAuthorsEndpoint -Method Post -Body $author -ContentType 'application/json'
$response | Format-Table

# --- Getting all authors ---
$response = Invoke-RestMethod -Uri $apiAuthorsEndpoint
$response | Format-Table

# --- Getting author by id ---
$response = Invoke-RestMethod -Uri "$apiAuthorsEndpoint/1"
$response | Format-Table

# --- Posting book ---
$book = @{
    Title = "Can't Hurt Me";
    Isbn = "9781544507859";
    ReleaseDate = "2018-11-15";
    Rating = 4;
    AuthorIds = @(1);
} | ConvertTo-Json
$response = Invoke-RestMethod -Uri $apiBooksEndpoint -Method Post -Body $book -ContentType 'application/json'
$response | Format-Table

# --- Posting book ---
$book = @{
    Title = "Can't Hurt Me";
    Isbn = "9781544507859";
    ReleaseDate = "2018-11-15";
    Rating = 4;
    AuthorIds = @(1);
} | ConvertTo-Json
$response = Invoke-RestMethod -Uri $apiBooksEndpoint -Method Post -Body $book -ContentType 'application/json'
$response | Format-Table

# --- Posting book ---
$book = @{
    Title  = "Never Finished";
    Isbn = "9781544534077";
    ReleaseDate = "2022-12-04";
    Rating = 5;
    AuthorIds = @(1);
} | ConvertTo-Json
$response = Invoke-RestMethod -Uri $apiBooksEndpoint -Method Post -Body $book -ContentType 'application/json'
$response | Format-Table

# --- Posting book ---
$book = @{
    Title  = "Be Useful";
    Isbn = "9780593655716";
    ReleaseDate = "2023-10-10";
    Rating = 4;
    AuthorIds = @(2);
} | ConvertTo-Json
$response = Invoke-RestMethod -Uri $apiBooksEndpoint -Method Post -Body $book -ContentType 'application/json'
$response | Format-Table

# --- Posting book ---
$book = @{
    Title  = "Atomic Habits";
    Isbn = "9783442178582";
    ReleaseDate = "2018-10-16";
    Rating = 5;
    AuthorIds = @(3);
} | ConvertTo-Json
$response = Invoke-RestMethod -Uri $apiBooksEndpoint -Method Post -Body $book -ContentType 'application/json'
$response | Format-Table

# --- Getting all books ---
$response = Invoke-RestMethod -Uri $apiBooksEndpoint
$response | Format-Table

# --- Getting all authors ---
$response = Invoke-RestMethod -Uri $apiAuthorsEndpoint
$response | Format-Table

# --- Getting book by id ---
$response = Invoke-RestMethod -Uri "$apiBooksEndpoint/1"
$response | Format-Table

# --- Posting borrower ---
$borrower = @{
    FirstName = "Alexander";
    LastName = "Carlsson";
    GotLoanCard = $true;
} | ConvertTo-Json
$response = Invoke-RestMethod -Uri $apiBorrowersEndpoint -Method Post -Body $borrower -ContentType 'application/json'
$response | Format-Table

# --- Posting borrower ---
$borrower = @{
    FirstName = "Johan";
    LastName = "Svensson";
    GotLoanCard = $false;
} | ConvertTo-Json
$response = Invoke-RestMethod -Uri $apiBorrowersEndpoint -Method Post -Body $borrower -ContentType 'application/json'
$response | Format-Table

# --- Getting all borrowers ---
$response = Invoke-RestMethod -Uri $apiBorrowersEndpoint
$response | Format-Table

# --- Getting borrower by id ---
$response = Invoke-RestMethod -Uri "$apiBorrowersEndpoint/1"
$response | Format-Table

# --- Posting loan (Fail) ---
$loan = @{
    BookId = 1;
    BorrowerId = 2;
    LoanDate = "2025-01-15";
} | ConvertTo-Json
$response = Invoke-RestMethod -Uri $apiLoansEndpoint -Method Post -Body $loan -ContentType 'application/json'
$response | Format-Table

# --- Posting loan ---
$loan = @{
    BookId = 1;
    BorrowerId = 1;
    LoanDate = "2025-01-15";
} | ConvertTo-Json
$response = Invoke-RestMethod -Uri $apiLoansEndpoint -Method Post -Body $loan -ContentType 'application/json'
$response | Format-Table

# --- Posting loan (Fail) --- 
$loan = @{
    BookId = 1;
    BorrowerId = 1;
    LoanDate = "2025-01-15";
} | ConvertTo-Json
$response = Invoke-RestMethod -Uri $apiLoansEndpoint -Method Post -Body $loan -ContentType 'application/json'
$response | Format-Table

# --- Updating loan by id ---
$loan = @{
    LoanDate = "2025-01-15"
    ReturnedDate = "2025-02-15";
} | ConvertTo-Json
$response = Invoke-RestMethod -Uri "$apiLoansEndpoint/1" -Method Put -Body $loan -ContentType 'application/json'

# --- Getting all loans ---
$response = Invoke-RestMethod -Uri $apiLoansEndpoint
$response | Format-Table

# --- Getting loan by id ---
$response = Invoke-RestMethod -Uri "$apiLoansEndpoint/1"
$response | Format-Table

# --- Deleting borrower by id ---
$response = Invoke-RestMethod -Uri "$apiBorrowersEndpoint/2" -Method Delete
$response = Invoke-RestMethod -Uri $apiBorrowersEndpoint
$response | Format-Table

# --- Deleting book by id ---
$response = Invoke-RestMethod -Uri "$apiBooksEndpoint/1" -Method Delete
$response = Invoke-RestMethod -Uri $apiBooksEndpoint
$response | Format-Table

# --- Deleting author by id ---
$response = Invoke-RestMethod -Uri "$apiAuthorsEndpoint/1" -Method Delete
$response = Invoke-RestMethod -Uri $apiAuthorsEndpoint
$response | Format-Table
