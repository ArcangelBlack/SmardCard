function showDownloadBtn(signatureToken){
	$("#loading").hide();	
	$('#download').show();
	$('#btnDownloadSignature').attr('href', 'downloadSignature?token=' + signatureToken);
}

function selectSignatureType(){	
	if($($('.wwFormTable tr')[2]).css('display') == 'none'){
		$($('.wwFormTable tr')[2]).show();
		$($('.wwFormTable tr')[3]).show();
	}
	else{
		$($('.wwFormTable tr')[2]).hide();
		$($('.wwFormTable tr')[3]).hide();
	}
}

function actionCall() {	
	$("#formulari").hide();
	$('#sendButton').hide();
	$("#loading").show();
	$('#base64Cert').val(cert);
}

function sendForm(){	
	$('#sendButton').hide();
	$("#loading").show();
}

function actionSignatureCall(){	
	$("#signatureForm").hide();
	$('#sendButton').hide();
	$("#loading").show();
}

function getPdfReport(){	
	$.ajax({
		url : "/signasuite/generatePdfReport",
	    type : "POST",
	    dataType : "HTML",
	    data : { 
	    	"htmlPdfReport" : $($("#content")[0]).html() 
	    },
	    success: function(data) { },
		error : function(a,b,c){}
	});
}

function changeValidationTime() {	
	if($('#verifyTimeContainer').css('display') == 'block') {		
		$('#verifyTimeContainer').hide();
	}
	else {		
		$('#verifyTimeContainer').show();
	}
}

function alterValidationTime() {
	if($("[type=radio][name=verifyTimeType]:checked").attr("value") == '2')	{		
		$('#verifyTimeContainer').show();
	}
	else {		
		$('#verifyTimeContainer').hide();
	}
}

function alterSignatureFormat(){
	if($("[type=radio][name=signatureFormatRadio]:checked").attr("value") == '2')	{		
		$('#signatureModeContainer').show();
	}
	else {		
		$('#signatureModeContainer').hide();
	}
}

function docTypeChange() {
	if($("[type=radio][name=docType]:checked").attr("value") == 'Original')	{		
		$('#formatContainer').show();
	}
	else {		
		$('#formatContainer').hide();
	}º
}

function docSignadorTypeChange() {
	if($("[type=radio][name=docType]:checked").attr("value") == 'Original')	{		
		$('#formatContainer').show();
	}
	else {		
		$('#formatContainer').hide();
	}
}



function selectDocType() {	
	// es busca aquest ID generat per struts2 amb el s:select	
	var selectedFormat = $('#uploadSignature_signatureType').find(":selected").text();	
	
	if(selectedFormat == 'XML/XAdES Detached' || selectedFormat == 'CAdES Detached' || selectedFormat == 'CMS Detached'){		
		$('#panel_signature_document').show();			
	}
	else{		
		$('#panel_signature_document').hide();			
	}
}

function selectDocTypeUpdate() {	
	// es busca aquest ID generat per struts2 amb el s:select
	var selectedFormat = $('#uploadUpdateSignature_signatureType').find(":selected").text();		
	if(selectedFormat == 'XML/XAdES Detached' || selectedFormat == 'CAdES Detached' || selectedFormat == 'CMS Detached'){		
		$('#panel_signature_document').show();	
	}
	else{		
		$('#panel_signature_document').hide();			
	}
}

function checkDocTypeChange(){
	$('input:radio[name=docType][value=Original]').attr('checked', true);
}


/* funcio que evalua si l'input conté la següent extensió */
function hasExtension(inputID, exts) {
    var fileName = document.getElementById(inputID).value;
    return (new RegExp('(' + exts.join('|').replace(/\./g, '\\.') + ')$')).test(fileName);
}

function selectFormatFromExtension(inputID) {
	var fileName = document.getElementById(inputID).value;
	var signMode;
	if(hasExtension(inputID, [".xml"]) || hasExtension(inputID, [".XML"])) {		
		signMode = '15';
	}
	else if(hasExtension(inputID, [".pdf"]) || hasExtension(inputID, [".PDF"])) {		
		signMode = '24';
	}
	else {		
		signMode = '26';
	}
		
	setAppletParam("signature_mode", signMode);
	// habilitem o deshabilitem les opcions de pdf en funció del mode de signatura
	if(signMode == "4" || signMode == "24" || signMode == "28") {
		$('#pdfSignatureConfig :input').removeAttr('disabled');
	}else{
		$('#pdfSignatureConfig :input').attr('disabled', true);
	}
	return signMode;
}

function alterCertificateSourceType() {		
	//console.log(document.appletcatcert);
	if($('input:radio[name=validateFromFile]:checked').val() == "magatzem")	{		
		$('#boto').click(function(event){ 
			event.preventDefault(); 
			validaCertificat(); 
		});
		
		// crida a la barra de progres		
		if(!$('#installedCertificatesDialog').is(":visible")){		
			$('#inputCertificateFile').hide();
			$('#progressBarContainer').show();			   
			$('#installedCertificatesDialog').show();
			$('#selectCert').prop('disabled', true);
			$('#progressBar').css('width', '30%').attr('aria-valuenow', 30);			
		}
	}
	else {			
		$('#boto').click(function(event){ 			
			$('#certificateForm').submit();	
		});
		document.location.reload();
		$('#progressBarContainer').hide();
		$('#installedCertificatesDialog').hide();
		$('#inputCertificateFile').show();		
	
		$('#selectCert').empty();			
		$('base64Cert').val('');		
	}
}

