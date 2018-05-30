/**
 * Variables per controlar el temps entre polling i el nombre de cops totals a preguntar
 * 5000 * 30 = 150 segons
 */
var pollingTime = 5000;
var pollingRepeats = 30;

/**
 * Funcio que inicia el proces de signatura amb el SC
 * Fa una crida POST i obte un token. A continuacio inicia un polling al signasuite
 * per anar preguntant si la signatura (identificada amb el token) existeix o no.
 * Quan el JNLP envia la signatura al SC, aquest la retorna a l'endpoint de signasuite
 * que esta escoltant i la desa a disc. Quan la funcio initTimer rep una resposta
 * JSON amb el camp created=true deixa de preguntar i habilita el boto de descarrega
 */

function signadorInit() {

    var formData;
    //if (window.FormData) {
    //    formData = new FormData(document.getElementById("signadorForm"));
    //}
    //
    var tokenId = "871ee6e1-a6fa-4e26-825c-a2f86af19356";
    var descripcio = "Prueba";
    var key = "3";
    var sign = "24";
    var docType = "6";
    var nomDoc = "AutoritzacioActivitat_0801180101_08052018_006";

    //var data = '';
    var hash = "SHA-1";
    var vsign = "true";
    var rSpc = "";
    var sField = "Signatura_Direccio";
    var cLevel = "0";
    var reason = "Aprovant el document";
    var location = "Barcelona";
    var sImg = "";
    var sRect = "";
    var sPN = "";
    var sRot = "0";
    var cVal = "true";

    var acas = "";
    var aoids = "";
    var salias = "";
    var scn = "";
    var stext = "";
    var psisVal = "false";
    var rnif = "";
    var kusg = "";

    var nenv = "false";
    var ndet = "false";
    var uris = "";
    var timestamp = "true";
    var tsa = "";
    var canonComments = "false";
    var protect = "false";

    var cmstsa = "";
    var cmstime = "false";

    var ciden = "";
    var cdes = "";
    var reference = "";
    var signer = "";
    var policy = "";
    var policyHash = "";
    var policyQualifier = "";
    var policyAlgorithm = "";

    formData = appletCfg({
        keystore: key, idToken: tokenId, signature: sign, docType: docType, nomDoc: nomDoc, document: document, descripcio: descripcio,
        visibleSign: vsign, reservedSpace: rSpc, signField: sField, certLevel: cLevel, reason: reason, location: location, signImg: sImg, signRectangle: sRect,
        signPageNum: sPN, signRotation: sRot, checkVal: cVal, allowedCas: acas, allowedOids: aoids, selectedAlias: salias, selectedCN: scn, selectedText: stext,
        psisValidation: psisVal, requiredNif: rnif, enveloping: nenv, detached: ndet, uris: uris, timestamp: timestamp, tsaUrl: tsa, canon: canonComments, protect: protect,
        cmsTimestamp: cmstime, cmsTsa: cmstsa, cidentifier: ciden, cdescription: cdes, reference: reference, signer: signer, policy: policy, policyHash: policyHash,
        policyQualifier: policyQualifier, policyAlgorithm: policyAlgorithm, hash: hash, keyUsage: kusg
    });

    //


    if (!isValidForm()) {

        $("#loading").show();
        $('#sendButton').hide();

        var options = {
            type: 'POST',
            url: 'https://signador.aoc.cat/signador/nativa?id=871ee6e1-a6fa-4e26-825c-a2f86af19356',
            dataType: 'html',
            cache: false,
            contentType: false,
            processData: false,
            error: function (data) {
                //console.log("error iniciant procés: " + data );
                $('div.alert').remove();
                $('<div class="alert alert-danger"><ul><li>Error realitzant la signatura, si us plau torneu a intentar-ho. Si el problema persisteix poseu-vos en contacte amb el Suport a Usuari del Consorci AOC.</li></ul></div>').insertAfter($('h1.page-header'));
            },
            success: function (data) {
                if (data.substring(0, 4) == 'http') {
                    //window.open(data);
                    window.location = data;
                }
                else {
                    $("<div class='alert alert-danger'><ul><li>S'ha produït un problema en la connexió amb el Signador Centralitzat</li></ul></div>").insertAfter($('h1.page-header'));
                }
                $("#loading").hide();
                $('#sendButton').show();
            }
        };

        if (formData) {
            options.data = formData;
            $.ajax(options);
        } else {
            $("#signadorForm").ajaxSubmit(options);
        }
    }

    return false;

}

var appletCfg = function (params) {
    return {
        //redirectUrl: serveiUrl.getRedirect(),
        //callbackUrl: serveiUrl.getReciveSignatureUrl(),
        token: params.idToken,
        descripcio: params.descripcio,
        applet_cfg: {
            keystore_type: params.keystore,
            signature_mode: params.signature,
            doc_type: params.docType,
            doc_name: params.nomDoc,
            document_to_sign: params.document,
            hash_algorithm: params.hash,
            pdf_cfg: {
                pdf_visible_signature: params.visibleSign,
                pdf_reserved_space: params.reservedSpace,
                pdf_signature_field: params.signField,
                pdf_certification_level: params.certLevel,
                pdf_reason: params.reason,
                pdf_location: params.location,
                pdf_signature_image: params.signImg,
                pdf_signature_rectangle: params.signRectangle,
                pdf_signature_page_number: params.signPageNum,
                pdf_signature_rotation: params.signRotation,
                pdf_show_adobe_sign_validation: params.checkVal
            },
            certs_cfg: {
                allowed_CAs: params.allowedCas,
                allowed_OIDs: params.allowedOids,
                selected_alias: params.selectedAlias,
                selected_CN: params.selectedCN,
                subject_Text: params.selectedText,
                psis_validation: params.psisValidation,
                required_nif: params.requiredNif,
                keyUsage: params.keyUsage
            },
            xml_cfg: {
                n_enveloping: params.enveloping,
                n_detached: params.detached,
                uris_to_be_signed: params.uris,
                includeXMLTimestamp: params.timestamp,
                xmlts_tsa_url: params.tsaUrl,
                canonicalizationWithComments: params.canon,
                protectKeyInfo: params.protect
            },
            cms_cfg: {
                timeStamp_CMS_signature: params.cmsTimestamp,
                cmsts_tsa_url: params.cmsTsa
            },
            ades_cfg: {
                commitment_identifier: params.cidentifier,
                commitment_description: params.cdescription,
                commitment_object_reference: params.reference,
                signer_role: params.signer,
                signature_policy: params.policy,
                signature_policy_hash: params.policyHash,
                signature_policy_qualifier: params.policyQualifier,
                signature_policy_hash_algorithm: params.policyAlgorithm
            }
        }
    };
};

/**
* Funcio per validar el formulari del signador centralitzat imitant el comportament de Struts2
* Decidit fer així ja que el validate() del Action dona problemes al ser cridat a traves d'ajax
*/
function isValidForm() {
    var result = true;
    $('div.alert').remove();
    if (!$('#folderDoc').is(':checked')) {
        if ($('#fileUpload').val() == '') {
            $('<div class="alert alert-danger"><ul><li>Cal adjuntar el fitxer/hash a signar</li></ul></div>').insertAfter($('h1.page-header'));
            result = false;
        }
    }
    return result;
}

/**
 * 
 */
function checkMode() {
    var modeSign = $('#signature').val();
    if (modeSign == '24') {
        $('#pdfImgs').show();
    } else {
        $('#pdfImgs').hide();
    }
}

/**
 * 
 */
function checkDocumentFile() {
    if ($('#folderDoc').is(':checked')) {
        $('#inputCertificateFile').hide();
        $('#fileUpload').val('');
    } else {
        $('#inputCertificateFile').show();
    }
}

/**
 * 
 * @param pos
 */
function setPosicioSignatura(pos) {
    desmarcarImatges();
    marcaImatge(pos);
    var posicio;
    if (pos == "0") {
        //La signatura va a l'esquerra i a l'última pàgina
        posicio = "100 100 200 200";
    } else {
        if (pos == "1") {
            //La signatura va al centre i a l'última pàgina
            posicio = "250 100 350 200";
        } else {
            //La signatura va a la dreta i a l'última pàgina
            posicio = "400 100 500 200";
        }
    }
    $('#positionPDFSign').val(posicio);
}

/**
 * 
 */
function marcaImatge(pos) {
    var borderChecked = "2px solid red";
    if (pos == "0") {
        $('#imatgeEsquerra').css('border', borderChecked);
    } else if (pos == "1") {
        $('#imatgeCentre').css('border', borderChecked);
    } else {
        $('#imatgeDreta').css('border', borderChecked);
    }
}

/**
 * 
 */
function desmarcarImatges() {
    $('#imatgeEsquerra').css('border', '');
    $('#imatgeCentre').css('border', '');
    $('#imatgeDreta').css('border', '');
}

/**
 * 
 * @returns
 */
function openSignador() {
    var urlLink = "https://signador.aoc.cat";
    window.open(urlLink, "_blank");
}

/**
 * 
 * @returns
 */
function openInfo() {
    $('#mes').hide();
    $('#amagat').show();
    $('#modePermesos').show();
    $('#formatContainer').show();
}

/**
 * 
 * @returns
 */
function closeInfo() {
    $('#mes').show();
    $('#amagat').hide();
    $('#modePermesos').hide();
    $('#formatContainer').hide();
}