/*
*   Global js functions
*/

/*
*   Form validations
*/

if (self != top) {
	top.location.href = self.location.href;
}

emailRe = /^[a-zA-Z0-9\_\-]+[a-zA-Z0-9\.\_\-]*@([a-zA-Z0-9\_\-]+\.)+([a-zA-Z]{2,4}|travel|museum)$/
submitButtonOn="/img/decals/button_action.gif"
submitButtonOff="/img/decals/button_search.gif"
formValidStyle='"border","1px solid green"'
formInvalidStyle='"border","1px solid red"'

//=================================================================================================
//Message for Alert
var pub_contextmenu_message = "The entire content on this site is protected by copyright, trademark rights and database rights.\n No reproduction without the consent of the relevant owner.";
//=================================================================================================

function RightClickImage(){
    alert(pub_contextmenu_message);
    return false;
}

function initOverLabels () {
  if (!document.getElementById) return;      

  var labels, id, field;

  // Set focus and blur handlers to hide and show 
  // labels with 'overlabel' class names.
  labels = document.getElementsByTagName('label');
  for (var i = 0; i < labels.length; i++) {

    if (labels[i].className == 'overlabel') {

      // Skip labels that do not have a named association
      // with another field.
      id = labels[i].htmlFor || labels[i].getAttribute('for');
      if (!id || !(field = document.getElementById(id))) {
        continue;
      } 

      // Change the applied class to hover the label 
      // over the form field.
      labels[i].className = 'overlabel-apply';

      // Hide any fields having an initial value.
      if (field.value !== '') {
        hideLabel(field.getAttribute('id'), true);
      }

      // Set handlers to show and hide labels.
      field.onfocus = function () {
        hideLabel(this.getAttribute('id'), true);
      };
      field.onblur = function () {
        if (this.value === '') {
          hideLabel(this.getAttribute('id'), false);
        }
      };

      // Handle clicks to label elements (for Safari).
      labels[i].onclick = function () {
        var id, field;
        id = this.getAttribute('for');
        if (id && (field = document.getElementById(id))) {
          field.focus();
        }
      };

    }
  }
};

function hideLabel (field_id, hide) {
  var field_for;
  var labels = document.getElementsByTagName('label');
  for (var i = 0; i < labels.length; i++) {
    field_for = labels[i].htmlFor || labels[i].getAttribute('for');
    if (field_for == field_id) {
      labels[i].style.textIndent = (hide) ? '-100000px' :'0px';
      return true;
    }
  }
}
function disableThisSubmit(){
	// check and highlight fields;
	$ES('.required',this.parentForm).each(function(self, name) {
		if (self.isValid) 
			if (self.isValid()) 
			{
				if (self.makeValid) self.makeValid();			
			}
			else 
			{
				if (self.makeInvalid) self.makeInvalid();			
			}
	});
	// display error if exists;
	
	$ES('.formError', this.parentForm).each(function(self, name) {
		self.setStyle('display', 'block');
	});
	
	return false;
}

function activateThisSubmit(el){
	if(el){
		el.src=submitButtonOn
		el.onclick=""		
	}
;
}

function createCheckboxes(){
	$$('input[type="checkbox"]').each(function(el,i){
		//console.log(el.type)
		//el.setStyle("display","none")
	})
}

/* To validate a form add the class 'required' to the submit button and any required field.
 * also add 'emailAdd' to email field
 * also add 'emailConf' to confirm email field
 * also add 'pass' to password field
 * also add 'passConf' to confirm password field
 */
// sets up forms for validation etc;
function iterateForms(){
	createCheckboxes()
	formsArray = $$('form')
	formsArray.each(function(self,index){
			formsArray[index].valid="true"
			self.elementArray = $ES('.required',formsArray[index])
			self.elementArray.each(function(self,name){
					self.parentForm = formsArray[index]	
				
					if(self.hasClass("action")){
						formsArray[index].childSubmit = self
						self.valid="true"
						self.src=submitButtonOff
						self.onclick=disableThisSubmit
						self.isValid=function() {return this.valid;}
					}else{
						
						// below lines removed so only changes when on blur (otherwise validates while typing)
						self.addEvent('keyup',validateWithoutHighlight) // this is done to change the submit button style without highlighting fields;
						//self.addEvent('change',validateThisField)
						if (self.type=="checkbox") self.addEvent('click',validateThisField)
						
						self.addEvent('blur',validateAndWarnIfNotValid)
						
						self.makeValid = function(){ // changes appearance to make valid;
							if (this.type=="checkbox") 
								this.parentNode.className='checkboxDiv';
							//else this.setStyle("border","1px solid green")
							this.highlightInvalid = true;
						}
						self.makeInvalid = function(){ // changes appearance to make invalid;
							if (this.type=="checkbox")
								this.parentNode.className='checkboxRequired';
							//else this.setStyle("border","1px solid red")
							this.highlightInvalid = true;
						}
						self.isValid = bindFieldValidator(self);
						once="true"	

						//self.fireEvent("keyup")
					}
					if(!self.isValid()){
						formsArray[index].valid="";	
						self.valid="false"
					}else{
				
					}
			})		
			if(formsArray[index].valid=="true"){
				activateThisSubmit(formsArray[index].childSubmit)
				return true;
			}else{
				return false;
			}
	})
	once="";
}

// validates forms;
function validateForms() {
	formsArray = $$('form');
	formsArray.each(function(self,index){
			formsArray[index].valid="true"
			self.elementArray = $ES('.required',formsArray[index])
			self.elementArray.each(function(self,name){
				if (self.isValid) 
					if(!self.isValid()) 
							formsArray[index].valid="";	
			})	
				
			if(formsArray[index].valid=="true"){
				activateThisSubmit(formsArray[index].childSubmit)
				return true;
			}else{
				formsArray[index].childSubmit.src=submitButtonOff
				formsArray[index].childSubmit.onclick=disableThisSubmit
				return false;
			}
	})
}


function validateAndWarnIfNotValid() {
	validateThisField.call(this);
	warnIfThisNotValid.call(this);
	// if this is email address form, validate also conf field;
	if (this.hasClass("emailAdd") && this.parentForm.emailConfEl) 
	{
		if (this.parentForm.emailConfEl.highlightInvalid) 
			validateAndWarnIfNotValid.call(this.parentForm.emailConfEl);
	}
	// if this is a password form, validate also conf field;
	if (this.hasClass("pass") && this.parentForm.passwordConfEl) 
	{
		if (this.parentForm.passwordConfEl.highlightInvalid) 
			validateAndWarnIfNotValid.call(this.parentForm.passwordConfEl);
	}	
	
}
function warnIfThisNotValid(){
	if(!this.valid){
		this.makeInvalid()
		this.parentForm.childSubmit.src=submitButtonOff
	}
}


// -- function bound to element; checks if is valid;
function bindFieldValidator(el) {
	if (el.type=="checkbox") el.label=el.title; 
	else el.label=el.getPrevious().innerHTML+"..."	;


	if(el.hasClass("emailAdd")){
		return function() {
			this.parentForm.emailVal = el.value;
			return (emailRe.test(this.value));
			}

	}else if(el.hasClass("emailConf")){	
		el.parentForm.emailConfEl = el;
		return function() {
			return (this.parentForm.emailVal == this.value && emailRe.test(this.value));
			}		
		
	}else if(el.hasClass("pass")){	
		return function() {
			this.label = this.getPrevious().innerHTML+"...";
			this.parentForm.passVal = this.value;
			if ((this.type!="checkbox"&&(this.value!="" && this.value!=el.label))||(this.type=="checkbox"&&this.checked))
				return true;
			else return false;
			}
		
	}else if(el.hasClass("passConf")){	
		el.parentForm.passwordConfEl = el;
		return function() {
			if ((this.parentForm.passVal==this.value)&&(this.value!="" && this.value!=this.label))
				return true; 
			else return false;
			}
	}else{
		return function() {
			if ((this.type!="checkbox"&&(this.value!="" && this.value!=this.label))||(this.type=="checkbox"&&this.checked))
				return true; 
			else return false;		
			}
	}
}

function validateWithoutHighlight() {
	if (this.isValid() && this.highlightInvalid) this.makeValid();
	
	this.valid = this.isValid();
	
	if(!once) validateForms();
}


function validateThisField(){
	if (this.isValid()) this.makeValid();
	else this.makeInvalid();
	
	this.valid = this.isValid();
	
	if(!once) validateForms();
}




/*
*  	End form validations
*/

/*
*   Textbox clear function
*/

var textBox = new Class({
    initialize: function(newBox, label){
        this.id = newBox.id;
        this.DOMNode = newBox
        this.value = newBox.value;
        this.label = label+"...";
    },
    
    setLabel: function(){
        var value = this.value;
        
        if(value == ""){    
            $(this.id).value=this.label;
        }
    }
}); 

function readyTextBoxes(){
    var inputTxt = document.getElements("input[type=text]");
    
    if(inputTxt.length > 0){
        for(var i=0;i<inputTxt.length;i++){
            var currentBox = inputTxt[i];
            var myBox = new textBox(currentBox, currentBox.getPrevious().innerHTML);     
            myBox.setLabel();
            currentBox.onfocus = function(){
                checkTextValue(this);
            }
            
            currentBox.onblur = function(){
                resetTextValue(this);
            }
        }
    }
}

function checkTextValue(textBox) {
    var textValue = textBox.value;
    var labelValue = textBox.getPrevious().innerHTML+"..."
    
    if(textValue == labelValue) {
        textBox.value = "";
    }
}

function resetTextValue(textBox) {
    var labelValue = textBox.getPrevious().innerHTML
    if(textBox.value == ""){
		//textBox.type="text"
        textBox.value = labelValue+"...";
    }
}


/*
*   END Textbox clear function
*/

/*
*   homepage SlideShow
*/

function homeSlideShow(){
    if($("homeSlideShow")){
        var images = [];
		var links = [];
        var slideShowImages = $$("#homeSlideShow li img")
        var slideShowLinks = $$("#homeSlideShow li a")
        for(var i=0;i<slideShowImages.length;i++){
            images.push(slideShowImages[i].src);
			if(slideShowLinks.length){
				links.push(slideShowLinks[i].href);
			}else{
				links="";
			}
        }
        if(images.length>0){startSlideShow(images,links)}
    }
}

function startSlideShow(myImages,myLinks){
    var slideFrame = $("homeSlideShow").getFirst();
    
    myShow = new Slideshow(slideFrame, {hu: '', links: myLinks, images: myImages});
}

/*
*   END homepage SlideShow
*/

/*
*   Print for calendar page
*/
function readyPrint(){
    if($$(".util_print").length>0){
        var printLink = $$(".util_print")[0].getLast();
        
        printLink.onclick=function(){
           window.print();
           return false;
        };
    }
}
/*
*   END Print for calendar page
*/

/*
*   Tickets & Travel page
*/
function readyTNT(){
    if($("otherPackageOptions")){
        var checkBox = $("otherPackageOptions");
        
        checkBox.onclick=function(){
            revealOverlay();
            readyOtherPackages();
        };
    }
}

function revealOverlay(){
    $("tntOverlay").setStyle("display", "block");
}

function hideOverlay(){
    $("tntOverlay").setStyle("display", "none");
}

function readyOtherPackages(){
    var overlayOptions = $$("#tntOverlay label")
    
    if(overlayOptions.length>0){
        for(var i=0; i<overlayOptions.length;i++){
            var currentPackage=overlayOptions[i];
            
            currentPackage.onclick=function(){
                setService(this.lastChild);
                hideOverlay();
            };
        }
    }
}

function setService(chosenPackage){
    var OPO=$("otherPackageOptions");
    
    OPO.nextSibling.nodeValue=chosenPackage.nodeValue;
}
/*
*   END Tickets & Travel page
*/

function getQueryVariable(variable) {
	var query = window.location.search.substring(1);
	var vars = query.split("&");
	for (var i=0;i<vars.length;i++) {
		var pair = vars[i].split("=");
		if (pair[0] == variable) {
			return pair[1];
		}
	} 
	return "";
}

function LCase(Value) {
	return Value.toString().toLowerCase();
}

function Len(Expression) {
	return Expression.toString().length;
}

function Right(Str, Length) {
	return Str.substring(Len(Str) - Length, Len(Str));
}

function InStr(Start, String1, String2, Compare) {
	if (Start > Len(String1)) return 0;
	if (Len(String2) == 0) return Start;
	if (Compare == 1) {String1 = LCase(String1); String2 = LCase(String2);}
	if (Start > 1) {
		var index = Right(String1, Len(String1) - Start + 1).indexOf(String2)
		if (index == -1) {return 0;} else {return index + Start;}
	} else {
		return String1.indexOf(String2) + 1
	}
}

function SessionArrayItem(g, n, s, l, sesionID, sessionTypeID) {
    var re;
    this.GPName = g;
    this.SessionName = n;
    this.SessionID = sesionID;
    this.SessionTypeID = sessionTypeID;
    this.start = new Date(s);
    this.end = new Date;
    this.end.setTime(this.start.getTime() + l * 60 * 1000); // in minutes ...
    //		this.end.setTime(this.start.getTime() + l * 1000); // in seconds ...
    return this;
}

function LinkRedirect(sURL, sFromDate, sToDate, redirectPath) {
	SetCookie('tickets_and_travel_redirect_2007', sURL, 9999);
	SetCookie('tickets_and_travel_fromdate', sFromDate, 9999);
	SetCookie('tickets_and_travel_todate', sToDate, 9999);
	window.open(redirectPath,'tandt_window','')
}


/*
*   Load Actions
*/
window.addEvent('domready', homeSlideShow);
//window.addEvent('domready', readyTextBoxes);
window.addEvent('domready', iterateForms);
window.addEvent('domready', readyPrint);
window.addEvent('domready', readyTNT);
window.addEvent('domready', initOverLabels);


function firefoxForceRerender(){
	// we need to trigger the rerender slightly in the future to give
	// the rendering threads a chance to sort themselves out
	setTimeout( firefoxForceRerenderTrigger, 10 );
}

function firefoxForceRerenderTrigger(){
	// we need to do something to the page to trick Firefox into rerendering it
	// just create a new SPAN, insert it at the end of the document and then
	// immediately delete it again
	var dummyNode = document.createElement("span");
	document.body.appendChild( dummyNode );
	Element.remove( dummyNode );
	
	if ($("ctrlFlash")) {
	    // $("ctrlFlash").style.display="none";
	    $("ctrlFlash").style.display="block";
	}
}	