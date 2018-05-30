/**
 * 
 */
function OS(){
	this.init();
}

//-----------------------------
//	Constants 
//-----------------------------

/**
 * 
 */
OS.prototype.init = function(  ){
	
}

/**
 * 
 */
OS.prototype.isWindows = function(  ){
	return ( navigator.appVersion.indexOf('Win') != -1 );
}

/**
 * 
 */
OS.prototype.isMac = function(  ){
	return ( navigator.appVersion.indexOf('Mac') != -1 );
}

/**
 * 
 */
OS.prototype.isUnix = function(  ){
	return ( navigator.appVersion.indexOf('X11') != -1 );
}

/**
 * 
 */
OS.prototype.isLinux = function(  ){
	return ( navigator.appVersion.indexOf('Linux') != -1 );
}
