download dotnet cli version 7.0
-----------------------------------------------------------

Extensions
-----------------------------------------------------------
1. C# -> Turn On Async Completion, Import Completion, Organize Imports On Format, useModernNet
2. C# Extensions -> Private member prefix = "_" turn off use this for ctor assignment 
3. Material Icon Theme
4. NuGet Gallery
5. SQLite
-----------------------------------------------------------

Project Setup
-----------------------------------------------------------
1. create project directory (lfiApp)
2. go inside lfiApp and run command to add solution file 
    cd lfiApp
    dotnet new sln
3. Create webapi project using command
    dotnet new webapi -n API
4. Add API folder to the solution file using command 
    dotnet sln add API
5. To run api execute command
    dotnet run 
    dotnet watch run | to enable auto reload on change
    dotnet run -lp https | to run from https profile
    dotnet watch run -lp https
6. Excluding certail folders not usefull for developmetn
    Go to setting -> Search exclude -> add pattern -> **/bin, **/obj
----------------------------------------------------------


Debugger
----------------------------------------------------------
If you can't find launch.json file inside .vscode folder from
command pallet search for .Net: Generate Assests for build and degub

Inside that we have two debugging option 
    1. .NET Core Launch (web) : To use this we need to stop API server and we can lauch process with debugger 
    2. .NET Core Attach       : Allow to use debugger to the currently running process.

Add breakpoint where you want to start debugging from and start.

Adding Entities Folder and creating our first Entity AppUser
------------------------------------------------------------
namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
    }
}

==
Properties must start with capital letter
Keep Id and UserName as it is, would be helpful later on.
------------------------------------------------------------


Entity Framework (DbContext Class) (Section 2: Lec 11)
-------------------------------------------------------------
Object Relational Mapper(ORM)
It translates our code into sql commands that can update our tables in db

AppUser ---- Entity Framework (DbContext Class) ---- AppUser Table in Database

Acts as bridge between code and database.
---------------
To download it install NuGet Gallery extension

Make sure to match the version of the package with the .Net version

Microsoft.EntityFrameworkCore.Sqlite -> as we are going to use sqlite for database for now
Microsoft.EntityFrameworkCore.Design -> as we are going to create database using code we need to use this package to design database

----------------
Now Create new folder inside API folder (Data)
Inside Data folder create DataContext class that inherits from DbContext class
To use DataContext class through out the application we need to register it as service to the program.cs file

// Program.cs
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

Now define the DefaultConnection in appsettings.Development.json
{
  "Logging": {
    ...
  },
  "ConnectionStrings": 
  {
    "DefaultConnection": "Data source=lfiApp.db"
  }
}

------------------
Now to create migrations (code that would create sql commands for c# to create database) we need to install tool dootnet-ef
As it is not provided in NuGet Gallery, we need to install it using cli.
run command
    dotnet tool install --global dotnet-ef --version 7.0.1
    dotnet tool list -g | to get the list of tools added

-------
Now to add migration run command: dotnet ef migrations add [name]
    dotnet-ef migrations add InitialCreate -o Data/Migrations | would create migrations inside Data -> Migrations 
    dotnet-ef database update | updates the database using migration files.


-------------------------------------------------------------
Adding a new API Controller

1. Create BaseApiController.cs inside controllers]
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        
    }
}

2. Now inherit this BaseApiController for new controller that we create suppose UsersController
    
    As we want to work with database we need to inject DataContext inside the consturctor and store
    its instance in _context property of the UsersController class.

    [HttpGet], [HttpGet("{id}")] are the verbs that defines the endpoints to the api more

namespace API.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }
        [HttpGet]
        public ActionResult<IEnumerable<AppUser>> GetUsers()
        {
            var users = _context.Users.ToList();
            return users;
        }
        [HttpGet("{id}")]
        public ActionResult<AppUser> GetUser(int id)
        {
            var user = _context.Users.Find(id);
            return user;
        }
    }
}

3. Improvising API controller to use async await
    we are using async await so that a request is not blocked while waiting for the database to respond

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user;
        }

-------------------------------------------------------------
Source Control - Git, GitHub

1. initialize the git repo -> git init
1. Add gitignore file to API folder -> dotnet new gitignore
2. add "API/appsettings.json" to .gitignore as we will save secret keys and credentials here
3. continue with commit process as defined in github repo

-------------------------------------------------------------
CORS (Cross-Origin Resource Sharing)

By default we cannot send request to server in different origin 
as cors is not implemented on the server. Only request comming 
from same origin could be responded, to overrule this problem
we need to add cors service in the program.cs file.

builder.Services.AddCors();

app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

-------------------------------------------------------------
Authentication Basics
    1. Storing password to the database
    2. Dto (Data transfer object)
    3. Validation   
    4. Json Web Tokens (JWT)

----
Any application have users as one of the entity and along with that 
basic function of register and login. So our user entity must have 
password field but we cannot store the password directly to the database.
We need to hash the password and then we can save it. 

-----
For now we would add two properties to AppUser entity 
    PasswordHash
    PasswordSalt
Both of these are byte array and they would use certain 
package that we would download from the NuGet Gallery.


1. AppUser.cs
namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}

As we have changed the AppUser class we need to add those columns to database
For that we would need to create a new migration. But before that we need to 
stop the dotnet watch run and then add migration to the folder using command.. 
    run command: 
        dotnet-ef migrations add UserPasswordAdded 
    Now update the database: 
        dotnet-ef database update

2. AccountController.cs

The login and register functionalities comes under the account so we will 
create an AccountController and add routes to login and register 

Register route

// {{url}}/api/account/register?username=Runil&password=runil

public async Task<ActionResult<AppUser>> Register(string username, string password)

Register method takes two parameters username and password and if the request sends
parameters same as these then only it can parse those parameter but if it is sent 
in json-body then this would not work as request would contain object. So we need
to create DTO(Data transfer object). 

-----------
DTOs are used to take input from the body of the request 
But the major reason to use dtos is to filter out some field that we don't want 
to send with response example password. So using dtos allows us to send only those
properties that we are interested in.

1. Create DTOs folder inside API and create RegisterDto.cs file 

// RegisterDto.cs
namespace API.DTOs
{
    public class RegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

2. Instead of parameters directly use RegisterDto in register route.

    public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)

3. Before creating user we need to check if there is any user with same username 
    For that create another async method that is UserExists that looks like 

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

    // AccountController.cs 
    using System.Security.Cryptography;
    using System.Text;
    using API.Data;
    using API.DTOs;
    using API.Entities;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    namespace API.Controllers
    {
        public class AccountController : BaseApiController
        {
            private readonly DataContext _context;
            public AccountController(DataContext context)
            {
                _context = context;
            }

            [HttpPost("register")]
            public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
            {

                if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

                using var hmac = new HMACSHA512();

                var user = new AppUser
                {
                    UserName = registerDto.Username.ToLower(),
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                    PasswordSalt = hmac.Key
                };

                _context.Users.Add(user);

                await _context.SaveChangesAsync();
                return user;
            }

            private async Task<bool> UserExists(string username)
            {
                return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
            }
        }
    }

4. Adding Validation

Currently if we try to add an user with username = "" then also it would accept or password = ""
To validate these inputs we can specify the data DataAnnotations in the DTOs, we need both of the 
fields to be required so we can specify that as 

    // RegisterDto.cs 
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }

This will not allow the request to reach even the Register method route if it does not pass the 
Validation that is username or password is empty.

Note: This validation is working only because we have use [ApiController] attribute in the BaseApiController

---------
Login Endpoint

Same as RegisterDto for register endpoint, we would create LoginDto for login endpoint, we could have 
used the same RegisterDto for login endpoint but moving on to the further development, we would like to 
add more registration details, so we can assume that RegisterDto can be changed in future. So we would 
create LoginDto.

    // LoginDto.cs 
    public class LoginDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }

Login route needs to check, whatever the username has been passed to it exist or not in the database,
if exist whether the password passed to it matches with the original password. After these conditions 
are satisfied the response can be sent with success otherwise unauthorized response is sent to client.

As we have used hashing we need to hash the password sent by the client with the same hashing key (PasswordSalt) 
used to store the password in the database. And if the new computedHash mathes with the passwordHash stored
in database then the password is correct and we can send success response. 

    // AccountController (Login Route)
    [HttpPost("login")]
    public async Task<ActionResult<AppUser>> Login(LoginDto loginDto) 
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username);

        if (user == null) return Unauthorized("Invalid Username");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for(int i = 0; i < computedHash.Length; i++)
        {
            if(computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
        }

        return user;
    }


-------------------------------------JWT Json Web Tokens----------------------------------------------
AS APIs are stateless they do not store state about the user sign in information that means when user 
login by putting correct username and password and send request the api would authenticate the user as 
valid user and that's all the api do, now for the next request made by the client the api doesn't know 
whether it is from the same authenticated client or other as API do not store the state. To overcome this 
problem JWT is used JSON WEB TOKEN is used. 

Whenever the user make login request to the server along with authentication details, server generates a 
jwt that is sent along with response that is stored on client machine(Browser Storage) (localStorage) and 
now on the request made by the client that token is added to that request and using that token server 
authenticate the user and serves them.

---------------
Till now we were adding the services provided by the entity framework but now we are going to create our 
own service and add to our service container in program.cs file so that we can inject it to the controller. 
Whenever we create a service it is advised to create it's interface first and then implement that service.

1. Create Interfaces folder inside API and create interface ITokenService.

    // ITokenService
    using API.Entities;
    namespace API.Interfaces
    {
        public interface ITokenService
        {
            string CreateToken(AppUser user);
        }
    }

2. Create Services folder and add TokenSerivce.
    To implement TokenSerivce (jwt) we need another package
    -> System.IdentityModel.Tokens.Jwt

    // TokenSerivce.cs
    namespace API.Services
    {
        public class TokenService : ITokenService
        {   
            private readonly SymmetricSecurityKey _key;
            public TokenService(IConfiguration config)
            {
                _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
            }
            public string CreateToken(AppUser user)
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
                };
                var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
                var tokenDescriptor = new SecurityTokenDescriptor 
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(7),
                    SigningCredentials = creds
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
        }
    }

4. Adding TokenKey to the appsettings.Development.json 
    "TokenKey": "UnguessableKey that cannot be guessed"

3. Adding TokenSerivce to the service container in Program.cs file 
    builder.Services.AddScoped<ITokenService, TokenService>();

4. Now Inject TokenService in AccountController and instead of User we would
    return UserDto and AccountController would look like.

    // AccountController.cs
    namespace API.Controllers
    {
        public class AccountController : BaseApiController
        {
            private readonly DataContext _context;
            private readonly ITokenService _tokenService;

            public AccountController(DataContext context, ITokenService tokenService)
            {
                _tokenService = tokenService;
                _context = context;
            }

            [HttpPost("register")]
            public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
            {
                if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");
                using var hmac = new HMACSHA512();
                var user = new AppUser
                {
                    UserName = registerDto.Username.ToLower(),
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                    PasswordSalt = hmac.Key
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return new UserDto 
                {
                    Username = user.UserName,
                    Token = _tokenService.CreateToken(user)
                };
            }

            [HttpPost("login")]
            public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username);
                if (user == null) return Unauthorized("Invalid Username");

                using var hmac = new HMACSHA512(user.PasswordSalt);
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
                }
                return new UserDto 
                {
                    Username = user.UserName,
                    Token = _tokenService.CreateToken(user)
                };
            }

            private async Task<bool> UserExists(string username)
            {
                return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
            }
        }
    }

-> Now the register and login route would send the UserDto that contains username and token. 
    On the client side this token is saved and for any request sent the token is added in the 
    authentication header and this is how the user are authenticated.

------
Adding authentication middleware so we can actually authenticate user 

1. Adding nuget package Microsoft.AspNetCore.Authentication.JwtBearer
2. Add the service to the Program.cs file.. Finally it would look like 

    // Program.cs 
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddControllers();
    builder.Services.AddDbContext<DataContext>(options =>
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
    builder.Services.AddCors();
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenKey"])),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
    var app = builder.Build();
    app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();

Now we have added Authentication and Authorization middleware to the api,
authentication middleware would authenticate if the user is valid or not 
by authenticating the token, by authorization middleware we would decide 
if the user is authenticated then user is authorized the access for which 
routes.

We can use the attribute such as [Authorize], [AllowAnonymous], before the 
controller or endpoints as we did in UsersController.

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        ...

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()...

        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUser(int id)...
    }
}

-----------------------------
Extensions

Creating Extensions folder and adding
    1. ApplicationServiceExtensions => Holds the application related services
    2. IdentityServiceExtensions => Holds the identity related services

    Move code from Program.cs file to respected service extension and add 
    them to Program.cs file using a single line.

-----------------------------------------------------------------------------------------------------------
Error Handling
