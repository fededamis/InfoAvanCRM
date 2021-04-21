
function OnLoad(context)
{    
    var formContext = context._formContext; 
    var formType = formContext.ui.getFormType();  

    if (formType == 1) 
        return;
 
    formContext.ui.clearFormNotification("contactQualificationMean");
    var accoundId = formContext.data.entity.getId().replace("{","").replace("}","");
    var queryFilter = "?$select=info_qualification&$filter=_parentcustomerid_value eq " +accoundId;
    var contactsArray = {}; 

    Xrm.WebApi.retrieveMultipleRecords("contact", queryFilter).then(
        function success(result) {

            if (result == null || result.entities.length == 0)
                return;
            
            var sum = 0.0;

            for (var i = 0; i < result.entities.length; i++) {
                sum += result.entities[i].info_qualification;
            }                   

            var mean = sum/result.entities.length; 

            if (mean < 4.0) {
                var mensaje = "El promedio de las calificaciones de los contactos es inferior a 4.";
                formContext.ui.setFormNotification(mensaje, "WARNING", "contactQualificationMean");
            }
        },
        function (error) {            
            var error = "[OnLoad Error]: " + error.message;
            formContext.ui.setFormNotification(error, "ERROR", "contactQualificationMean");
        }
    );   
}