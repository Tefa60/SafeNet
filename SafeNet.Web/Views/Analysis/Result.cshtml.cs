using System;
using System.Data;

@model SafeNet.Web.Models.ViewModels.AnalysisResultVM

@{
    ViewData["Title"] = "Resultado del análisis";
}

< div class= "container mt-4" >
    < div class= "row justify-content-center" >
        < div class= "col-md-8" >

            < div class= "alert alert-@Model.ColorVeredicto text-center shadow" role = "alert" >
                < i class= "bi @Model.IconoVeredicto fs-1" ></ i >
                < h2 class= "mt-2 fw-bold" > @Model.Veredicto </ h2 >
                < p class= "mb-0" > Nivel de riesgo: < strong > @Model.NivelRiesgo </ strong ></ p >
            </ div >

            < div class= "card shadow mb-3" >
                < div class= "card-body" >
                    < h5 class= "card-title" > Explicación </ h5 >
                    < p class= "card-text" > @Model.Explicacion </ p >
                </ div >
            </ div >

            < div class= "card shadow mb-3" >
                < div class= "card-body" >
                    < h5 class= "card-title" > Mensaje analizado </ h5 >
                    < p class= "card-text text-muted fst-italic" > @Model.TextoAnalizado </ p >
                </ div >
            </ div >

            @if(Model.EsSimulacion)
            {
                < div class= "alert alert-warning" >
                    < i class= "bi bi-info-circle" ></ i >
                    < strong > Modo simulación activo.</ strong >
                    Este análisis fue realizado por el motor de detección local.
                </div>
            }

            < div class= "d-flex gap-2" >
                < a asp - action = "Index" class= "btn btn-primary" >
                    < i class= "bi bi-arrow-left" ></ i > Analizar otro mensaje
                </a>
            </div>

        </div>
    </div>
</div>