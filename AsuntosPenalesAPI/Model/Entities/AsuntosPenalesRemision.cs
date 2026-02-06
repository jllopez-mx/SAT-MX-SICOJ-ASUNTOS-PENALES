using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsuntosPenalesAPI.Model.ViewModels;

namespace AsuntosPenalesAPI.Model.Entities
{
    public class AsuntosPenalesRemision

    {
    public int id { get; set; }
    public int id_asunto_penal { get; set; }
    public int? id_unidad_administrativa_remite { get; set; } 
    public int? id_unidad_administrativa_recibe { get; set; } 
    public int? id_tipo_autoridad { get; set; }
    public string? numero_oficio{ get; set; }
    public int id_estado_tarea { get; set; }
    public int id_estado_procesal { get; set; }
    public string? usuario { get; set; }
    public DateTime fecha_oficio { get; set; }
    public DateTime fecha_modificacion { get; set; }
    public bool activo { get; set; }
    public int? id_unidad_administrativa_externa { get; set; } 

    

    }
}