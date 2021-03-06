﻿#Notes on ASP.NET Core Project

## Routing system

```
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Hello World!");
    });
});
```

## Middleware request pipeline: MW! Incoming > Hello > MW1.OutGoing
```
app.Use(async (context, next) =>
{
    logger.LogInformation("MW1: Incoming request");
    await next();
    logger.LogInformation("MW1: Outgoing response");
});

app.Run(async (context) =>
{
    await context.Response.WriteAsync("Hello Munuiiii");
});
```

## custom default file selection
```
DefaultFilesOptions defaultFiles = new DefaultFilesOptions();
defaultFiles.DefaultFileNames.Clear();
defaultFiles.DefaultFileNames.Add("demofile.html");
//----Serves the index.html / default.html files first
app.UseDefaultFiles(defaultFiles); 
//----Serve static files : UseDefaultFiles must be registered before UseStaticFiles Middleware
app.UseStaticFiles();
```

### custom default file name from file directory also

```
FileServerOptions fileServer = new FileServerOptions();
fileServer.DefaultFilesOptions.DefaultFileNames.Clear();
fileServer.DefaultFilesOptions.DefaultFileNames.Add("demofile.html");
//fileServer.DefaultFilesOptions.DefaultFileNames.Add("img/juice.jpg");
//----Serves the index.html / default.html files first
app.UseFileServer(fileServer);
//UseFileServer = UseDirectoryBrowser+UseDefaultFiles+UseStaticFiles
```

## When the error won't be thrown
```
app.Run(async (context) =>
{
    //The error won't be thrown if the default file is found previously
    throw new Exception("Error occurred");
    await context.Response.WriteAsync("Hello Munuiiii");
});
```

## number of lines to show before the error line
```
var devExceptionOptions = new DeveloperExceptionPageOptions()
{    
    SourceCodeLineCount = 10
};
//contains stack trace, cookies, headers, query string
app.UseDeveloperExceptionPage(devExceptionOptions);
```

## Environment variables


1. Development : Debug, Unhandlex errors, Unminified scripts, 
    developer exception page

2. Staging: Identical to production, identity deployment related issues, 
    B2B apps service provider EndToEnd testing, 
    minified scripts loaded, generic error message

3. Production : Configured security, custom error page


> ASPNETCORE_ENVIRONMENT in the properties>launchsettings.json
> ASPNETCORE_ENVIRONMENT comes from the json file or windows system variable
    any option is okay

IHostingEnvironment Type: IsDevelopment, IsStaging, IsProduction

check custom environment name: env.IsEnvironment("CustomEnv") // UAT, QA etc

When not specified, runtime default name is: Production


> custom error page redirect: IApplicationBuilder.UseExceptionHandler("/Error")

## MVC

```
Request >     Controller     DataSource
              /        \       |
            View   >   Model  <|

Response <

domain.com/controller/method/parameter
```

## doesn't require to specify a path, default: Home/Index
> app.UseMvcWithDefaultRoute();

* If not found, runt he next middleware data like, WriteAsync() data

* use mvc middleware in .net 3.0 by modifying this service > services.AddMvc(options => options.EnableEndpointRouting = false);


## Dependency injection : procides loose coupling, easy testing system

> This is a tightly coupled system, we can not easily change the implementation of the interface later
` IEmployeeRepository _repository = new MockEmployeeRepository()`

> When there are a large number of controllers, using a single instance of interface is helpful, no need to worry about the implementation of the interface
` services.AddSingleton<IEmployeeRepository, MockEmployeeRepository>(); `

* AddSingleton : Created at the first request and that instance is used through the whole application lifetime

* AddTransient : Creates instance for every request 
* AddScoped : Creates instance once per request within the scope

## Send custom object as json type from the controller
```
public ObjectResult Details()
{
    return new ObjectResult(_repository.GetEmployee(1));
}
```

### JSon data type format

```
// Pascal casing
services.AddControllersWithViews().
AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});
```

### Send XML format data, default is json
```
services.AddMvc(options => options.EnableEndpointRouting = false)
         .AddXmlSerializerFormatters();
//set the postman request parameter: (Accept: application/xml)
```

## Customize view discovery in asp net core mvc

> MVC searchs the view in this format
```
InvalidOperationException: The view 'Details' was not found. The following locations were searched:
/Views/Home/Details.cshtml
/Views/Shared/Details.cshtml
/Pages/Shared/Details.cshtml
```

> custom view name, not following the method name
* //--File inside the same controller directory name : Views>Home>Test
` return View("Test"); `

> --file inside the views folder : Views>Test>File
`return View("../Test/TestFile");`

> --file inside the project directory : EmployeeManagement>TestViews>ViewFile

```
return View("TestViews/TestViewFile.cshtml"); or 
return View("~/TestViews/TestViewFile.cshtml"); or
return View("~/Views/Home/Index.cshtml", model);

View(object model) // file with the same action method name
View(string viewName) //custom view name
View(string viewName, object model) // custom view and model data
```


## Pass data to view

* Options: ViewData, ViewBag, StronglyTypedView
* Indication to razor that we're switching html to c#

### ViewData:

1. inside the controller
```
ViewData["Employee"] = model;
ViewData["PageTitle"]= "Employee model";
```

2. inside a cshtml view
```
<h3> @ViewData["PageTitle"] </h3>
@{
    var employee = ViewData["Employee"] as EmployeeManagement.Models.Employee;
}
<p>@employee.Name</p>
```

It's a weakly typed, key value store, dynamically resolved at runtime
No compiletime check, No IntelliSense, Misspelling might happen


### ViewBag:
A wrapper around viewdata, DynamicProperties, No compiletime type checking,

No IntelliSense,

Cannot perform runtime binding on a null reference

1. inside the controller
```
ViewBag.Employee = employeeModel;
ViewBag.PageTitle = "Employee model";
```

2. inside a cshtml view
```
<h2>Page title: @ViewBag.PageTitle</h2>
<p>Name: @ViewBag.Employee.Name </p>
```


## StronglyTypedView: Recommended 
It uses: StronglyTyped Model Object

1. Inside the Controller
```
ViewBag.PageTitle = "Employee model";
return View(employeeModel);
```

2. for the View
```
@model EmployeeManagement.Models.Employee
<h2>Page title: @ViewBag.PageTitle</h2>
<p>Name: @Model.Name </p>
<p>Department: @Model.Department </p>
```

* Define view using the @model directive
* To acces the data, we use @Model directive 
* Contains compile time checking and IntelliSense


## View models : DTO / Data Transfer Object

Collection of one or more properties, Parent model

1. Inside the Controller
```
var homeDetailsViewModel = new HomeDetailsViewModel()
{
    Employee = _repository.GetEmployee(1),
    PageTitle = "Employee model"
};
```

2. for the View
```
@model EmployeeManagement.ViewModels.HomeDetailsViewModel
<h2>Page title: @Model.PageTitle</h2>
<p>Name: @Model.Employee.Name </p>
```

## List / Table view

list model direcive: `@model IEnumerable<EmployeeManagement.Models.Employee>`

looping
```
@foreach(var employee in Model)
{
<tr>
    <td>@employee.Id</td>
    <td>@employee.Name</td>
    <td>@employee.Department</td>
</tr>
}
```


> Layout view is the Master page

### SECTION
Section can be optional
Section layout view is rendered at the location where RenderSection() method is called

before body tag:-------

```
@if(IsSectionDefined("Scripts"))
{
    @RenderSection("Scripts", required: true)
}
```

inside the page area:-------- drag the script file to get the automatic script tag

```
@section Scripts {
    <script src="~/js/script.js"></script>
}
```


### ViewStart

Reduces code redundancy and improves maintainability
```
@{
    Layout = "_Layout";
    //Layout = "~/Views/Shared/_Layout.cshtml";
}
```


Use multiple layout file using the name: Layout = "_Layout2";
```
@{
    if (User.IsInRole("Admin"))
    {
        Layout = "_AdminLayout";
    }
    else
    {
        Layout = "_Layout2";
    }
}
```

ViewStart file code is executed before the view.
It follows hierarchical Execution and can be used in many places.

* View > _ViewStart.html
* View > Home > _ViewStart.html

We can place the file inside any of the subfolder, keeping the main one


## ViewImports

` _ViewImports.cshtml ` is placed in the Views folder
Used to include the common namespaces using @using directives
Other supported directived:
` @addTagHelper @removeTagHelper @tagHelperPrefix @model @inherits @inject `
It follows hierarchical Execution.

` @inject IEmployeeRepository _repository > use it inside a view to access some data `

## Routing

> conventional routing
host_address/controller/action/parameter

> Attribute Routing
```
[Route("Home")]         //direct approach
[Route("[controller]")] //token method

[Route("[controller]/[action]")] //for controller

[Route("[controller]/[action]/[id?]")] //? for optional parameters
```

### Ading botstrap

files can be installed using the libman.json file.
Add the file name, it will show intellisense and save the file to pull the actual script files.

## Tag helpers

```
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers

<a href="@Url.Action("details", "home", new { id = employee.Id })" class="btn btn-primary">View 1</a>

@Html.ActionLink("View2", "details", "home", new { id = employee.Id })

<a href="/home/details/@employee.Id" class="btn btn-primary">View3</a>
```

//using asp.net core, the URL generates bases on the app Route template
//i.e; company/controller/action will be auto generated in this approach
` <a asp-controller="home" asp-action="details" asp-route-id="@employee.Id" class="btn btn-primary">View4</a> `


//each time the image changes in the server, a new hash is generated to prevent loading from the cache ; it's cache busting behaviour
src="~/img/img_avatar1.png" asp-append-version="true" 

### Environment Tag Helper

```
<environment include="Development"> </environment>
<environment include="Staging,Production"> </environment>
<environment exclude="Development"> </environment>
```

integrity checks SRI/Subresource Integrity Check is a security feature. 

browser calculate a hash value for the file and matched with the integrity value.
if the file is not the same / altered, the hash value won't match & the browser will block the download.

if the browser failed to download the file from cdn, fallback and download another.

check by this way: asp-fallback-test-class="sr-only" ; 

test-class check if this class is available, to make sure the css file is here, if not download the fallback one

turn off integrity check from a fallback source: asp-suppress-fallback-integrity



### Model binding
To bind the request data to the controller action method params,
model binding looks for the data in the HTTP req in this sequence:
Form value > Route Values > Query String


` IActionResult > RedirectToActionResult + ViewResult `

Builtin validation attributes: RegularExpression, Required, Range, MinLength, MaxLength, Compare


## AddSingleton vs AddScoped vs AddTransient

HomeController creates an instance of IEmployeeRepository.
View @inject this repository also. ( @inject IEmployeeRepository _repository )

1. AddSingleton : single instance served for every HTTP request throughout the application
* When a request is received. HomeController.IEmployeeRepository instance is served to the View.IEmployeeRepository.

```
List.Elements = 3
HttpRequest.Add > List.Elements++ : 4
HttpRequest.Count = 4 (last 3 + 1)

HttpRequest.Add > List.Elements++ : 5
HttpRequest.Count = 5 (last 4 + 1)
```

2. AddScoped : gets the same instace within the scope of a given http request but a new instance across different HTTP request. InitialRequest+CurrentRequest
```
List.Elements = 3
HttpRequest.Add > List.Elements++ : 4
HttpRequest.Count = 4 (initial 3 + 1)

HttpRequest.Add > List.Elements++ : 5
HttpRequest.Count = 4 (initial 3 + 1)
```

3. AddTransient : A new instance is provided for every request, request scope (same http request / different) doesn't matter.
```
List.Elements = 3
HttpRequest.Add > List.Elements++ : 4
HttpRequest.Count = 3 (initial 3 )

HttpRequest.Add > List.Elements++ : 5
HttpRequest.Count = 3 (initial 3 )
```



## Entity Framework Core

ORM - Object Relational Mapper
MS Official Data Access Platform

Code First Approach: 
1. Create ApplicationDomain Classes & DbContext classes
2. Based on those, EF Core creates the database and the database tables

Database First Approach : when we have an existing database
1. Create the Domain and DbContext classes based on the existing database


## Application Layers
1. Presentation Layer
2. Business Layer
3. Data Access Layer

> Required packages

* Microsoft.EntityFrameworkCore.SqlServer
* Microsoft.EntityFrameworkCore.Relational
* Microsoft.EntityFrameworkCore.Design
* Microsoft.EntityFrameworkCore.Tools
* Microsoft.EntityFrameworkCore

` Microsoft.EntityFrameworkCore.SqlServer installs the Relational, EFCore packages `

## DbContext related

`AddDbContextPool<AppDbContext>` provides pooling. Everytime the context is requested, asp.net core returns the previously created instance. 

Helps to implement Loosely Coupled system and Efficient Repository Pattern system.

### Migration

Migration
```
Help: 
Get_Help about_entityframeworkcore
Get_Help Add-Migration

Add-Migration Initial -Context "BasicWebAppDbContext" -StartupProject "BasicWebApp" -Project "BasicWebApp.Data"

//Apply the latest migration or an old one to Revert the migration
Update-Database [[-Migration] <String>] [-Context <String>] [-Project <String>] [-StartupProject <String>] [<CommonParameters>]
Update-Database

//Remove the migration that is not yet updated to database
Remove-Migration [-Force] [-Context <String>] [-Project <String>] [-StartupProject <String>] [<CommonParameters>]
Remove-Migration

Update-Database Initial -Context "BasicWebAppDbContext" -StartupProject "BasicWebApp" -Project "BasicWebApp.Data"

```

### Extension method definition
```
//definition
public static class ModelBuilderExtensions
{
    public static void LoadSeedData(this ModelBuilder modelBuilder) { }
}

//use case
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.LoadSeedData();
}

```

### Upload a file 

Basic example for a image file in the www/img folder. 

```
// IWebHostEnvironment hostingEnvironment
private string ProcessUploadedImage(EmployeeCreateViewModel employeeModel)
{
    string uniqueFileName = null;
    if (employeeModel.Photo != null)
    {
        string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "img"); //www folder
        uniqueFileName = Guid.NewGuid().ToString() + "_" + employeeModel.Photo.FileName;
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
        using(var fileStream = new FileStream(filePath, FileMode.Create) )
        {
            employeeModel.Photo.CopyTo(fileStream);
        }
    }

    return uniqueFileName;
}
```


### 404 type error

1. Resource with the specified id does not exist.
```
if(employee == null)
{
    Response.StatusCode = 404;
    return View("EmployeeNotFound", id);
}
```
2. The URL does not match with any route

> Centralized 404 Error handling
1. UseStatusCodePages - default error text returns
2. UseStatusCodePagesWithRedirects - custom error page view

Initially when the URL does not matches with any of the items, 
the initial request gives a 302-NotFound status code. 
So, this middleware component issues another get request.
The redirected 404 page is a 200 type response.
The original URl changes also.

3. UseStatusCodePagesWithReExecute

ReExecute the pipeline and returns the right status code.
Returns a 404 type response taht can be seen in the browser's network tab.
This middleware intercepts the response and provides the error page and the 404 type error.
The URL stays the same.

* Global exception handling

## Logging

Logging provider stores or displays logs

Grouping the error category by the class in this way bu injecting ILogger service:

` ILogger<ErrorController> logger; `

```
//string interpolation
logger.LogError($"The path {exceptionDetails.Path} threw an exception {exceptionDetails.Error}");

//logged error
warn: EmployeeManagement.Controllers.ErrorController[0]
      404 error occurred. Path = /gg and query string = 
```

* Logging using NLog.Web.AspNetCore
nlog.config file contains the required logging config for nlog.
enable copy to bin folder: nlog.config > properties - Copy to Output Directory = Copy if newer

* ASP.NET Core LogLevel config:
Trace=0, Debug=1, Information=2, Warning=3, Error=4, Critical=5, None=6
example: `Microsoft.Extensions.Logging.LogTrace()`

* when the log level `"Default": "Warning"` , then any log message >= Warning will show.

* Custom log level for specific file: inside the appsettings > Logging > LogLevel
` "EmployeeManagement.Models.SqlEmployeeRepository: : "Trace" `

* filter logging by logging provider
1. Logging > Debug > LogLevel - for the Debug type
2. Logging > LogLevel - for the rest of the types

### launchsettings.json

* profiles
1. IIS Express - run using IISExpress server
2. EmployeeManagement - run using `dotnet run` from cmd 

## Get valus from the appsettings.json file - 2 types

```
public UserController(IConfiguration iConfig)  
{  
   configuration = iConfig;  
} 

string dbConn = configuration.GetSection("MySettings").GetSection("DbConnection").Value;  
string dbConn2 = configuration.GetValue<string>("MySettings:DbConnection");

```

## Identity

It's a membership system with CRUD ops for User accounts
Account confirmation
Authentication, Authorization
Password recovery
Auth. with SMS
External login providers like, Google, Facebook

1. Derive the context from `Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext`

DbContext > IdentityUserContext > IdentityDbContext

2. Identity service configure

```
services.AddIdentity<IdentityUser, IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>(); //use EfCore to get user & role info
```

`IdentityUser` contains the user's login info like ID, Email, Password hash.
We can make a custom class and derive it from IdentityUser to add `Gender` or similar kinds of attributes.
`IdentityUser > table: AspNetUsers`

3. Add the authentication middleware before the mvc middleware
`app.UseAuthentication();`

4. Create Identity tables


## User and Signin manager

* UserManager<IdentityUser>
1. CreateAsync
2. DeleteAsync
3. UpdateAsync, etc.

* SignInManager<IdentityUser>
1. SignInAsync
2. SignOutAsync
3. IsSignedIn, etc.

SignInAsync.isPersistent sets the SessionCookie or a Permanent cookie. 
to set the session cookie: isPersistent: false

* The basic idea of user creation and signing option
```
var user = new IdentityUser
{
    UserName = regModel.Email,
    Email = regModel.Email
};

var result = await userManager.CreateAsync(user, regModel.Password);

if(result.Succeeded)
{
    await signInManager.SignInAsync(user, isPersistent: false);
    return RedirectToAction("index", "home");
}
```

* login by password
```
signInManager
    .PasswordSignInAsync(loginModel.Email, loginModel.Password, loginModel.RememberMe, false)
```

To change the default login page... 
` services.ConfigureApplicationCookie(options => options.LoginPath = "/Account/LogIn"); `

* change the password strongness level
1. using IdentityOptions

```
services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredUniqueChars = 3;
});
```

2. Configure the IdentityOptions
```
services.AddIdentity<IdentityUser, IdentityRole>(options => 
{
    options.Password.RequiredLength = 4;
}
```

## Auth

Authentication : Identifying the user
Authorization : Identifying what the user can do and can't do

[Authorize] : only the logged in users can access
[AllowAnonymous] : everyone can access

*. `RequireAuthenticatedUser` is one of the core requirements embedded within the `AuthorizationPolicyBuilder` class which is the tool to use to create an authorization policy
*. The method just add the `DenyAnonymousAuthorizationRequirement` requirement to the policy requirements collection

*. To apply [Authorize] attribute globally on all controllers and controller actions throughout your application modify the code in ConfigureServices method of the Startup class.
```
.AddMvcOptions(options =>
{
    var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
})
```


## ASP.NET Core 2.2 Version of MVC

1. Inside the ConfigureServices method
```
//we want the user to be authenticated
services.AddMvc(options =>
{
    var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
    options.EnableEndpointRouting = false;
})
.AddXmlSerializerFormatters();

```
2. Configure method
```
//app.UseMvc() for the default option
app.UseMvc(routes =>
{
    routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
});
```

### Pass query string or the ReturnUrl

1. in the @ directive area, after the layout definition
`var returnUrl = @Context.Request.Query["returnurl"];`

2. In the form tag
`<form asp-controller="account" asp-action="login" method="post" asp-route-returnurl="@returnUrl">`

3. Receive the URL
`public async Task<IActionResult> Login(LoginViewModel loginModel, string returnUrl = "")`

4. Check the URL and redirect - to prevent the Redirect Attack
```
if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
{
    return Redirect(returnUrl);
}
```


### ASP NET Core client side validation

Validate using jquery, takes the serverside validation attributes and implement client-side validation 

jquery > jquery.validate > jquery.validate.unobtrusive


*. Remote valdation


### ASP NET Core Roles

User and role mapping
AspNetUsers table, AspNetRoles table >> AspNetUserRoles

User mapping with the roles
1. Role based attribute, the user can be of any one of the roles: `[Authorize(Roles = "Admin,Manager")]`

2. The user must be Admin & Manager

```
[Authorize(Roles = "Admin")]
[Authorize(Roles = "Manager")]
```

### Enforce ON DELETE NO ACTION in entity framework core

1. AspNetUsers 
2. AspNetRoles
3. Users & Role mapping data table, both are foreign keys : AspNetUserRoles

By default, deleting a role record, user relation gets removed from AspNetUserRoles.
that is, ON DELETE CASCADE (delets the child rows). 
We don't want a role to be deleted if there are users asigned. 
Set OnDelete:Restrict

### Custom error page



