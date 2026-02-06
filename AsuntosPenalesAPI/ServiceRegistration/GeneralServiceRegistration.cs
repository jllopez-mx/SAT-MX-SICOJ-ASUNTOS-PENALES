using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsuntosPenalesAPI.Model.DAO.Repository;
using AsuntosPenalesAPI.Model.DAO.ServicesDAO;
using AsuntosPenalesAPI.Model.IDAO.IRepository;
using AsuntosPenalesAPI.Model.IDAO.IServiceDAO;
using Sicoj.Utils.Extentions;

namespace AsuntosPenalesAPI.ServiceRegistration
{
    public static class GeneralServiceRegistration
    {
        public static IServiceCollection AddGeneralServices(
            this IServiceCollection services
        )
        {
            services.AddScoped<IAsuntosPenalesOficialPartesRepository, AsuntosPenalesOficialPartesRepository>();
            services.AddScoped<IArchivosAsuntosPenalesRepository, ArchivosAsuntosPenalesRepository>(); 
            services.AddScoped<IAsuntosPenalesAdministradorRepository, AsuntosPenalesAdministradorRepository>();
            services.AddScoped<IAsuntosPenalesAbogadoRepository, AsuntosPenalesAbogadoRepository>();
            services.AddScoped<IAsuntosPenalesAdministradorGRepository, AsuntosPenalesAdministradorGRepository>();
            services.AddScoped<IAsuntosPenalesAdministradorUARepository, AsuntosPenalesAdministradorUARepository>();
            services.AddScoped<IAsuntosPenalesRemisionRepository, AsuntosPenalesRemisionRepository>(); 
            services.AddScoped<IAsuntosPenalesRepository, AsuntosPenalesRepository>(); 
            services.AddScoped<IAsuntosPenalesModificacionRepository, AsuntosPenalesModificacionRepository>();
            services.AddScoped<IAsuntosPenalesDescartarRepository, AsuntosPenalesDescartarRepository>();
            services.AddScoped<IAsuntosPenalesOficialPartesService, AsuntosPenalesOficialPartesService>();            
            services.AddScoped<IAsuntosPenalesAdministradorService, AsuntosPenalesAdministradorService>();
            services.AddScoped<IAsuntosPenalesAbogadoService, AsuntosPenalesAbogadoService>(); 
            services.AddScoped<IAsuntosPenalesAdministradorGService, AsuntosPenalesAdministradorGService>(); 
            services.AddScoped<IAsuntosPenalesAdministradorUAService, AsuntosPenalesAdministradorUAService>(); 
            services.AddScoped<IGenericService, GenericService>();
            services.AddScoped<ApiService>();
     
            return services;
        }
    }
}