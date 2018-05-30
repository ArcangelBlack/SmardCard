/*
 * Author: aciffone@aoc.cat
 */

// FUNCIONS DE RETORN QUE POT GENERAR LAPPLET ***********************

function onSignOK(signature) {
	//alert('Signatura generada correctament:\n');
	// posem la sortida també al textarea
	//writeOutputAppletJS(signature);
	//download file from JS using FileSave.js
	// metode per descarregar la signatura des de Javascript
	downloadSignature(signature);
}

function downloadSignature(signature) {
	
	var signatureTypeValue;
	var signatureTypeText;
	
	if($('form input[name=signatureFormatRadio]:checked').val() == '1'){
		signatureTypeValue = parseInt(selectFormatFromExtension('fileToSignPath'));
		//signatureTypeValue = 15;		
		if(signatureTypeValue == 15){
			signatureTypeText = 'XAdES-T detached ';
		}
		else if(signatureTypeValue == 24){
			signatureTypeText = 'PDF signat ';			
		}
		else{
			signatureTypeText = 'CAdES-T detached ';
		}		
	}
	else{
		signatureTypeValue = $("#signatureMode").val();
		signatureTypeText =	$("#signatureMode option:selected").text();
	}	
	
	//var signatureTypeValue = $("#signatureMode").val();
	
	var newDate = new Date();
	var datetime = newDate.getDate() + "-" + (newDate.getMonth()+1) + "-" + newDate.getFullYear();
	var extension = "";
	var blob = null;
			
	if(signatureTypeValue == 1 || 
	   signatureTypeValue == 2 ||
	   signatureTypeValue == 21 ||
	   signatureTypeValue == 22 ||
	   signatureTypeValue == 25 ||
	   signatureTypeValue == 26 ){
				// XML cases	
				extension = ".p7s";	
				//var binSignature = b64_to_utf8(signature);
				var binSignature = base64DecToArr(signature);
				blob = new Blob([binSignature], {type: "application/octet-stream"});	
	}
	else if(signatureTypeValue == 5 ||
			signatureTypeValue == 6 ||
			signatureTypeValue == 7 ||
			signatureTypeValue == 9 ||
			signatureTypeValue == 10 ||
			signatureTypeValue == 11 ||
			signatureTypeValue == 13 ||
			signatureTypeValue == 14 ||
			signatureTypeValue == 15
			){		
				// CMS i CAdES cases		
				extension = ".xml";		
				blob = new Blob([signature], {type: "text/xml;charset=utf-8"});			
	}
	else if(signatureTypeValue == 24) {
		// PDF case		
		extension = ".pdf";	
		var binSignature = base64DecToArr(signature);
		blob = new Blob([binSignature], {type: "application/pdf"});	
	}
	
	var signatureFileName = signatureTypeText + datetime + extension;		
	saveAs(blob, signatureFileName);
}


function base64DecToArr (sBase64, nBlocksSize) {

	  var
	    sB64Enc = sBase64.replace(/[^A-Za-z0-9\+\/]/g, ""), nInLen = sB64Enc.length,
	    nOutLen = nBlocksSize ? Math.ceil((nInLen * 3 + 1 >> 2) / nBlocksSize) * nBlocksSize : nInLen * 3 + 1 >> 2, taBytes = new Uint8Array(nOutLen);

	  for (var nMod3, nMod4, nUint24 = 0, nOutIdx = 0, nInIdx = 0; nInIdx < nInLen; nInIdx++) {
	    nMod4 = nInIdx & 3;
	    nUint24 |= b64ToUint6(sB64Enc.charCodeAt(nInIdx)) << 18 - 6 * nMod4;
	    if (nMod4 === 3 || nInLen - nInIdx === 1) {
	      for (nMod3 = 0; nMod3 < 3 && nOutIdx < nOutLen; nMod3++, nOutIdx++) {
	        taBytes[nOutIdx] = nUint24 >>> (16 >>> nMod3 & 24) & 255;
	      }
	      nUint24 = 0;

	    }
	  }

	  return taBytes;
}

function b64ToUint6 (nChr) {

	  return nChr > 64 && nChr < 91 ?
	      nChr - 65
	    : nChr > 96 && nChr < 123 ?
	      nChr - 71
	    : nChr > 47 && nChr < 58 ?
	      nChr + 4
	    : nChr === 43 ?
	      62
	    : nChr === 47 ?
	      63
	    :
	      0;

}

function onMultiSignOK(signature) {
	/*alert('Signatura generada per l\'event final:\n' + signature);*/
}
function onSignCancel() {
	alert('Procés cancel·lat per l\'usuari');
}
function onSignError(msg) {
	alert('Error durant la generació de la signatura: \n' + msg);
}

function onLoadError(msg) {
	alert('Error durant la càrrega de l\'applet:\n' + msg);
}

/**
 * Callback de l'applet amb el path del fitxer seleccionat
 * @param path
 * 		Ruta del fitxer seleccionat
 */
function onFileUpload(path){
	$("#fileToSignPath").val(path);
}

/**
 * Retorn de l'applet amb el certificat del signant
 * @param cert
 * 		Certificat del signant en Base64
 */
function getSignCert(cert){
	//alert("El certificat del signant en Base64 és: \n" + cert);
	// posem la sortida també al textarea
	writeOutputAppletJS(cert);
	
}

/**
 * Funció que captura el callback de l'applet amb el certificat codificat en Base64
 * @param certificate
 * 		Certificat seleccionat en base64
 */
function onCertExported(certificate){
	// injecta a l'input hidden el valor cel cert en b64
	$('#base64Cert').val(certificate);	
	// fa el submit per cridar l'Action
	$('#certificateForm').submit();	
}

// *********************** FUNCIONS DE RETORN QUE POT GENERAR LAPPLET 

// INVOCACIONS A LAPPLET ********************************************

/**
 * Crida per setejar de forma dinamica amb js els parametres de lapplet.
**/
function setAppletParam(name,value) {

	//alert("SET APPLET PARAM. NAME : " + name + " VALUE: " + value);
	try{
		document.appletcatcert.set(name,value);
	}catch(Exception){
		// chrome, safari i opera
		document.appletcatcert[1].set(name,value);
	}
}

/**
  * En el mode embedded (parametre embedded = true), aquesta es la funcio que reb el retorn de lapplet indicant que la carrega
  * del mateix sha realitzat correctament, i rebent com a parametre els alias de les claus del magatzem configurat
  * amb els que podrem realitzar la signatura. (substitueix el onSignLoad de lapplet amb mode popup).
  * Aquest es nomes un exemple de com omplir un combo amb els alies seleccionats, pero es poden pintar com es vulgui.
  *
**/
function onReturnCerts(value){
	$('#progressBar').css('width', '60%').attr('aria-valuenow', 60);   
	
	if(value[0] != undefined && value[0].length != 1){
		for(var i=0;i<value.length;i++){
			$("#selectCert").append("<Option value='" + value[i] + "'>" + value[i] + "</option>");	
		}
	}else{
		$("#selectCert").append("<Option value='" + value + "'>" + value + "</option>");	
	}
	$('#progressBar').css('width', '99%').attr('aria-valuenow', 99);   
	$('#progressBarContainer').fadeOut(1000);
	$('#selectCert').prop('disabled', false);
		
	hideLoadPanel();
}

/**
 * 	Funció que recupera el certificat del signant i fa la crida a lapplet per a realitzar la signatura 
**/
function signar(){
	// format per defecte, cal seleccionar en funcio de l'extensió del fitxer a signar
	/*if($('#signatureFormatRadio').val() == 1){
		if XML
		$('#signature_mode').val('15');
		else
		$('#signature_mode').val('26');
	}
	else{
		
	}*/
	
	// si esta seleccionat format "per defecte"
	if($('form input[name=signatureFormatRadio]:checked').val() == '1'){
		selectFormatFromExtension('fileToSignPath');
	}
	// si volem signar urls setegem els valors
	if($("#doc_type").val() == "6"){
		setAppletParam("document_to_sign",$("#urlToSign").val());
	}
	
	// recuperem l'alies del cert amb el que signarem
	var alies = $("#selectCert").val();
	// invoquem lapplet
	try{
		document.appletcatcert.signFromJS(alies);
	}catch(Exception){
		// chrome, safari i opera
		document.appletcatcert[1].signFromJS(alies);
	}
}

function validaCertificat(){
	$('#sendButton').hide();
	$("#loading").show();
	exportCert();	
}

/**
 * Funció per a la selecció del fitxer o carpeta a signar
 */
function inputFileOnChange(){	
	document.appletcatcert.openFolderDialog();	
}

/**
 * Funció per a obtenir el certificat amb el que s'ha realitzat la signatura
 */
function exportSignCert(){
	// invoquem l'applet per a obtenir el certificat del signant
	try{
		document.appletcatcert.getSignCertificate();
	}catch(Exception){
		// chrome, safari i opera
		document.appletcatcert[1].getSignCertificate();
	}
}


/**
 * Funció que permet recuperar el certificat seleccionat en Base64
 */
function exportCert(){
	
	var alies = $("#selectCert").val();	
	// invoquem lapplet passant lalies seleccionat
	try{
		document.appletcatcert.exportCertFromJS(alies);
	}catch(Exception){
		// chrome, safari i opera
		document.appletcatcert[1].exportCertFromJS(alies);
	}
}

// ******************************************** INVOCACIONS A LAPPLET

/**
 * Deshabilita el background i mostra un div amb les opcions de configuració avançada
 */
function advancedSettings(){
	
	alert("advanced settings");
	// mostrem el div amb les opcions de config
    $('#advancedSettings').fadeIn(function() {
    	// deshabilitem el background:
    	$("body").append('<div id="backgroundDisable" class="modalOverlay">');
      });
}

/**
 * Amaga els advanced settings
 */
function closeAdvancedSettings(){
	
	// amaguem els advanced
	$('#advancedSettings').fadeOut(function() {
    	// habilitem la plana de nou
		$("#backgroundDisable").remove();
      });
}

/**
 * Mostra la sortida de l'applet
 */
function traceOutputAppletJS(){
	// primer comprovem si el js_event està habilitat, sino ho
	// esta ho fem automaticament
	var checked = $("#js_event").is(":checked");
	
	if(!checked){
		$('#js_event').trigger('click');
	}
	
	// mostrem el text area per a la sortida
	$("#outputApplet").css("display","block");
	
	// canviem el text del button
	$('#outputAppletBtn').text('Amaga sortida');
	
	// canviem la funció
	$('#outputAppletBtn').attr('href', 'javascript:hideOutputAppletJS();');
}

/**
 * Amaga la sortida de l'applet
 */
function hideOutputAppletJS(){
	// amaguem el text area per a la sortida
	$("#outputApplet").css("display","none");
	
	// canviem el text del button
	$('#outputAppletBtn').text('Mostrar sortida');
	
	// canviem la funció
	$('#outputAppletBtn').attr('href', 'javascript:traceOutputAppletJS();');
}

/**
 * Escriu el text al textarea
 * @param text
 */
function writeOutputAppletJS(text){
	$("#outputAppletText").val(text);
}

/**
 * Deshabilita el fons i mostra el panell de càrrega
 */
function showLoadPanel(){
	// deshabilitem el fons
	$("body").append('<div id="backgroundDisable" class="modalOverlay">');
	
	// mostrem el panell de carrega
	 var width = $(this).width();
	 var height = $(this).height();
	 
	 $("#report-loading").css({top: ((height / 2) - 25),left: ((width / 2) - 50)
	    }).fadeIn(200);
}

/**
 * Amaga el panell de càrrega i habilita el background
 */
function hideLoadPanel(){
	// amaguem el panell de càrrega
	$("#report-loading").fadeOut(1000);
	
	// habilitem el fons
	$("#backgroundDisable").remove();
}

/* Funcions afegides per el Signasuite */

