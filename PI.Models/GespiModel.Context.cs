﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PI.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class GPIEntities : DbContext
    {
        public GPIEntities()
            : base("name=GPIEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Titular> Titulares { get; set; }
        public virtual DbSet<Bitacora> Bitacora { get; set; }
        public virtual DbSet<ClasificacionDeViena> ClasificacionDeViena { get; set; }
        public virtual DbSet<ClassificacionDeNiza> ClassificacionDeNiza { get; set; }
        public virtual DbSet<Correlativos> Correlativos { get; set; }
        public virtual DbSet<Cronologia> Cronologia { get; set; }
        public virtual DbSet<Estatus> Estatus { get; set; }
        public virtual DbSet<Leyes> Leyes { get; set; }
        public virtual DbSet<LogosMarcas> LogosMarcas { get; set; }
        public virtual DbSet<LoteExpedientes> LoteExpedientes { get; set; }
        public virtual DbSet<Lote> Lotes { get; set; }
        public virtual DbSet<Mandatarios> Mandatarios { get; set; }
        public virtual DbSet<Marca> Marcas { get; set; }
        public virtual DbSet<Modulo> Modulos { get; set; }
        public virtual DbSet<Pais> Paises { get; set; }
        public virtual DbSet<PrioridadMarca> PrioridadMarcas { get; set; }
        public virtual DbSet<Productividad> Productividad { get; set; }
        public virtual DbSet<ProductosMarca> ProductosMarcas { get; set; }
        public virtual DbSet<TiposDeMarca> TiposDeMarca { get; set; }
        public virtual DbSet<TiposDeRegistro> TiposDeRegistro { get; set; }
        public virtual DbSet<Ubicaciones> Ubicaciones { get; set; }
        public virtual DbSet<VienaMarcas> VienaMarcas { get; set; }
        public virtual DbSet<AudiovisualAutor> AudiovisualAutores { get; set; }
        public virtual DbSet<ComposicionAutor> ComposicionAutores { get; set; }
        public virtual DbSet<DAResolucion> DAResoluciones { get; set; }
        public virtual DbSet<FonogramaArtista> FonogramaArtistas { get; set; }
        public virtual DbSet<Formularios> Formularios { get; set; }
        public virtual DbSet<GuionAutor> GuionAutores { get; set; }
        public virtual DbSet<Templates> Templates { get; set; }
        public virtual DbSet<Abandonos> Abandonos { get; set; }
        public virtual DbSet<Agente> Agentes { get; set; }
        public virtual DbSet<Clasificacion> Clasificaciones { get; set; }
        public virtual DbSet<Inventor> Inventores { get; set; }
        public virtual DbSet<IPC> IPC { get; set; }
        public virtual DbSet<Prioridad> Prioridades { get; set; }
        public virtual DbSet<Resolucion> Resoluciones { get; set; }
        public virtual DbSet<Titulos> Titulos { get; set; }
        public virtual DbSet<Areas> Areas { get; set; }
        public virtual DbSet<Anualidad> Anualidades { get; set; }
        public virtual DbSet<Rol> Roles { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<UsuarioPublico> UsuariosPublicos { get; set; }
        public virtual DbSet<Publicacion> Publicaciones { get; set; }
        public virtual DbSet<Favorito> Favoritos { get; set; }
        public virtual DbSet<GrupoExpediente> GrupoExpedientes { get; set; }
        public virtual DbSet<GrupoMiembro> GrupoMiembros { get; set; }
        public virtual DbSet<Grupo> Grupos { get; set; }
        public virtual DbSet<Session> Session { get; set; }
        public virtual DbSet<Sequence> Sequence { get; set; }
        public virtual DbSet<Auditoria> Auditoria { get; set; }
        public virtual DbSet<Expediente> Expedientes { get; set; }
        public virtual DbSet<AporteAudiovisual> AporteAudiovisual { get; set; }
        public virtual DbSet<DerechoDeAutor> DerechoDeAutor { get; set; }
        public virtual DbSet<LiterariasyArtisticas> LiterariasyArtisticas { get; set; }
        public virtual DbSet<ObrasMusicalesyEscenicas> ObrasMusicalesyEscenicas { get; set; }
        public virtual DbSet<Productor> Productores { get; set; }
        public virtual DbSet<Solicitante> Solicitantes { get; set; }
        public virtual DbSet<Autor> Autores { get; set; }
        public virtual DbSet<FonogramaTituloDeObra> FonogramaTituloDeObras { get; set; }
        public virtual DbSet<Permiso> Permisos { get; set; }
        public virtual DbSet<TitularesDeLaMarca> TitularesDeLaMarca { get; set; }
        public virtual DbSet<Patente> Patentes { get; set; }
        public virtual DbSet<CronologiaDocto> CronologiaDoctos { get; set; }
        public virtual DbSet<MandatarioDeLaMarca> MandatarioDeLaMarcas { get; set; }
        public virtual DbSet<Renovacion> Renovaciones { get; set; }
        public virtual DbSet<Anotacion> Anotaciones { get; set; }
        public virtual DbSet<TitularDeLaPatente> TitularesDeLaPatente { get; set; }
        public virtual DbSet<TitularEnPatentes> TitularesEnPatentes { get; set; }
        public virtual DbSet<AnotacionEnExpedientes> AnotacionEnExpedientes { get; set; }
        public virtual DbSet<GacetaSecciones> GacetaSecciones { get; set; }
        public virtual DbSet<Avisos> Avisos { get; set; }
        public virtual DbSet<Gaceta> Gaceta { get; set; }
    }
}
