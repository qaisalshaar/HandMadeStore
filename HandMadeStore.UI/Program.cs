
using HandMadeStore.DataAccess.Data;
using HandMadeStore.DataAccess.Repository;
using HandMadeStore.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using HandMadeStore.Utilities;
using Stripe;
using HandMadeStore.UI.Hubs;
using Microsoft.Extensions.Localization;
using HandMadeStore.UI;
using System.Globalization;
using Microsoft.Extensions.Options;
using NuGet.Protocol;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("ConKey")

    ));
// add stripe sevices

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
// add MailJetSettings services
builder.Services.Configure<MailJetSettings>(builder.Configuration.GetSection("MailJetSettings"));

builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// add facebook + google login and register

//builder.Services.AddAuthentication().AddFacebook(options =>
//{
//    options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
//    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
//}).AddGoogle(options =>
//{
//    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
//    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
//});

builder.Services.AddAuthentication().AddGoogle(options =>
{
      options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
       options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});



//builder.Services.AddScoped<ICategoryRepositry, CategoryRepositry>();
// · ‘€Ì· ’›Õ«  »⁄œ «÷«›… «·—Ê· RazorePages
builder.Services.AddRazorPages();

// «÷«›… Signalr
builder.Services.AddSignalR();

// «÷«› Â ··Ê’Ê· «·Ï «”„ «·„” Œœ„ œ«Œ· «·—Ì»Ê” —Ì
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ··Ê’Ê· «·Ï DbInitializer
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

// · ‘€Ì· «· «Ì„Ì· ”Ì‰œ—
builder.Services.AddSingleton<IEmailSender,EmailSender>();
builder.Services.AddTransient<IEmailSenderNew,EmailSenderNew>();


//  ⁄œÌ· «·„”«— ›Ì Õ«·… «‰ «·„” Œœ„ ·„ Ìﬁ„ »«· ”ÃÌ· »«·„Êﬁ⁄

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});




//start add session  contain string or number

builder.Services.AddSession(option =>
{
    // „œ… »ﬁ«¡ «·”Ì‘‰ 
    option.IdleTimeout=TimeSpan.FromMinutes(100);
    // «·Ê’Ê· ··”Ì‘‰ „‰ «·ﬂ·«Ì‰  ”«Ì  ›ﬁÿ
    option.Cookie.HttpOnly = true;
    // «·ŒÌ«— «·„ ⁄·ﬁ »«·ﬂÊﬂÌ
    option.Cookie.IsEssential = true;

});
//End add session  contain string or number

// add loclaization
builder.Services.AddLocalization();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSingleton<IStringLocalizerFactory,JsonStringLocalizerFactory>();
builder.Services.AddMvc().AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat
    .Suffix)
    .AddDataAnnotationsLocalization(options =>
    {

        options.DataAnnotationLocalizerProvider = (type,factory)=>
        factory.Create(typeof(JsonStringLocalizerFactory));
        ;
    });


builder.Services.Configure<RequestLocalizationOptions>(options =>
{

    var supportedCultures = new[]
    {
 //new CultureInfo("ar-EG"),
         new CultureInfo(name:"en-US"),
         new CultureInfo(name:"ar-EG"),
       
        
     
    };
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(culture: supportedCultures[0]
        , uiCulture: supportedCultures[0]);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
// pipeline add stripe services
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:Secretkey").Get<string>();




// For Globolaization
var supportedCultures = new[] {  "en-US" , "ar-EG" };
 
var LocolaizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("en-US")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

// add it to middleware
app.UseRequestLocalization(LocolaizationOptions);






// «” œ⁄«¡ «·„ÌÀÊœ
SeedData();
// „”ƒ·… ⁄‰ «·‰Õﬁﬁ „‰ ÂÊÌ… «·„” Œœ„ «·–Ì ÌﬁÊ„ » ”ÃÌ· «·œŒÊ·
app.UseAuthentication();

// middle ware for Session
app.UseSession();
//
// „”ƒ·… ⁄‰ «·œŒÊ· «·Ï ’›Õ«  «·„Êﬁ⁄
app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");
// · ‘€Ì· ’›Õ«  «·„⁄ „œ… ⁄·Ï Identity
app.MapRazorPages();



var supportedCulture = new[] { "en-US", "ar-EG" };

app.UseRequestLocalization(r =>
{
   
    r.AddSupportedCultures(supportedCulture);
    r.AddSupportedUICultures(supportedCulture);
    //  ÕœÌœ «··€… «·«› —«÷Ì…
    r.SetDefaultCulture(supportedCulture[0]);
});


// «÷«›… —Ê· «·”ﬂ‰· «—  Signalr
// «·ﬂ·«Ì‰  Ì‘Ê› «·”Ì—›—
app.MapHub<ReviewsHub>("/Reviews");
app.MapHub<MessageHub>("/Message");



app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default1",
    pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}");

// add rotativa asp.net core for Server Side Reporting
// get wwwroot path
RotativaConfiguration.Setup(builder.Environment.WebRootPath);

app.Run();


void SeedData()
{
    using var scope = app.Services.CreateScope();
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    dbInitializer.Initialize().Wait();
}