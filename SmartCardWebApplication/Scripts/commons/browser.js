/**
 * 
 */
function Browser(){
	this.init();
}

//-----------------------------
//	Constants 
//-----------------------------

/**
 * 
 */
Browser.prototype.init = function(  ){
	
}

/**
 * 
 */
Browser.prototype.isOpera = function(  ){
	return (!!window.opr && !!opr.addons) || !!window.opera || navigator.userAgent.indexOf(' OPR/') >= 0;
}

/**
 * 
 */
Browser.prototype.isFirefox = function (){
	return typeof InstallTrigger !== 'undefined';
}

/**
 * 
 */
Browser.prototype.isSafari = function (){
	return Object.prototype.toString.call(window.HTMLElement).indexOf('Constructor') > 0;
}

/**
 * 
 */
Browser.prototype.isIE = function (){
	return /*@cc_on!@*/false || !!document.documentMode;
}

/**
 * 
 */
Browser.prototype.isEdge = function (){
	return !this.isIE() && !!window.StyleMedia;
}

/**
 * 
 */
Browser.prototype.isChrome = function (){
	return !!window.chrome && !!window.chrome.webstore;
}

/**
 * 
 */
Browser.prototype.isBlink = function (){
	return ( this.isChrome() || this.isOpera() ) && !!window.CSS;
}