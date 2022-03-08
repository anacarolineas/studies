using DevAna.Api.Data;
using DevAna.Api.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DevAna.Api.Configuration
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(opt =>
                opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddErrorDescriber<IdentityMessagesCustom>() // mensagens error customizadas
                .AddDefaultTokenProviders(); // tokens para identificação de e-mail, reset de senha

            #region JWT

            var appSettingsSection = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>(); //pegar os dados da classe AppSettings
            var key = Encoding.ASCII.GetBytes(appSettings.Secret); //enconding secret

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // padrão de validação token
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; //valida token

            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true; //espera uma chamada https, evita ataques 
                x.SaveToken = true; // http authentication properties salvar
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true, //valida se quem está emitindo é o mesmo quando recebo o token
                    IssuerSigningKey = new SymmetricSecurityKey(key), // criptografia de chave
                    ValidateIssuer = true,
                    ValidateAudience = true, // onde o token é válido
                    ValidAudience = appSettings.ValidoEm,
                    ValidIssuer = appSettings.Emissor
                };
            });

            #endregion

            return services;
        }
    }
}
