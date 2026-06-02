using System;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using static System.Collections.Specialized.BitVector32;

@model SafeNet.Web.Models.ViewModels.AnalysisInputVM

@{
    ViewData["Title"] = "Analizar mensaje";
}

< div class= "container mt-4" >
    < div class= "row justify-content-center" >
        < div class= "col-md-8" >
            < div class= "card shadow" >
                < div class= "card-header bg-primary text-white" >
                    < h4 class= "mb-0" >
                        < i class= "bi bi-shield-check" ></ i > SafeNet — Detector de Estafas
                    </h4>
                </div>
                <div class= "card-body" >
                    < p class= "text-muted" >
                        Pega aquí el mensaje, correo o texto sospechoso y SafeNet lo analizará por ti.
                    </p>

                    <form asp-action="Analyze" method="post">
                        @Html.AntiForgeryToken()
                        <div class= "mb-3" >
                            < label asp -for= "Texto" class= "form-label fw-bold" ></ label >
                            < textarea asp -for= "Texto"
                                      class= "form-control"
                                      rows = "6"
                                      placeholder = "Ejemplo: Felicitaciones! Ganaste un premio de S/5000. Haz click aqui para reclamar..." ></ textarea >
                            < span asp - validation -for= "Texto" class= "text-danger" ></ span >
                        </ div >
                        < div class= "d-grid" >
                            < button type = "submit" class= "btn btn-primary btn-lg" >
                                < i class= "bi bi-search" ></ i > Analizar ahora
                            </ button >
                        </ div >
                    </ form >
                </ div >
            </ div >
        </ div >
    </ div >
</ div >

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}