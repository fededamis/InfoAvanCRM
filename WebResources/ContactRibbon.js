
function CallApi(recordGuid)
{    
    if (recordGuid == null)
        return;    
    var parameters = {};
    parameters.ContactId = recordGuid.replace("{","").replace("}","");

    var req = new XMLHttpRequest();
    req.open("POST", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/info_HttpGetContactApi", true);
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.onreadystatechange = function() {
        if (this.readyState === 4) {
            req.onreadystatechange = null;
            if (this.status === 200) {
                                
                var results = JSON.parse(this.response);
                
                if (results == null)
                    return;
                
                if (results.Error != null) {
                    mensajeError = "[CallApi Error]: " + results.Error;
                    Xrm.Utility.alertDialog(mensajeError); 
                    return;
                }

                if (results.Resultado != null) {
                    mensajeResultado = "[CallApi] Se obtuvo respuesta de API: " + results.Resultado;
                    Xrm.Utility.alertDialog(mensajeResultado);
                    return;
                }                
            } else {
                Xrm.Utility.alertDialog(this.statusText);
            }
        }
    };
    req.send(JSON.stringify(parameters));    
}