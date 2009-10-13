var tymer = null;  // timeout variable;

function fixMacFFxBug(){
      if(navigator.userAgent.indexOf('Mac') != -1){
            if(navigator.appName == "Netscape"){
                  $$(".tertiaryNavItem ul li").each(
                        function(el){     
                              el.addEvent("mouseout",function(){
                                    yIndex = parseInt(this.getParent().scrollTop);
                                    this.getParent().scrollTo(0,yIndex+1);
                                    this.getParent().scrollTo(0,yIndex);
                              })
                        }
                  )
            }
      }
}

function positionSlider(){
	this.position=this.parentSpan.offsetLeft + this.xWidth - 6
	this.slider.setStyle("left",this.parentNav.getPosition().x + this.position)
	this.slider.setStyle("top",this.getPosition().y+2)
	this.slider.setStyle("z-index",1000)		
}

function repositionAllSliders(){
	var es = $$(".liveSlider");
	if (es) es.each(function(el){if (el.reposition) el.reposition()})
}


Element.Events.extend({
	'wheelup': {
		type: Element.Events.mousewheel.type,
		map: function(event){
			event = new Event(event);
			if (event.wheel >= 0) this.fireEvent('wheelup', event)
		}
	},
 
	'wheeldown': {
		type: Element.Events.mousewheel.type,
		map: function(event){
			event = new Event(event);
			if (event.wheel <= 0) this.fireEvent('wheeldown', event)
		}
	}
});

function addWheeling(el){
	currentList=el;
	scrollSpeed=10
	currentList.mySlide.step=0;
	currentList.mySlide.set(currentList.mySlide.step);
	if (!currentList.wheeled){
				currentList.addEvents({
							'wheelup': function(e) {
								e = new Event(e).stop();
								currentList.mySlide.set(currentList.mySlide.step-scrollSpeed);
							},
						 
							'wheeldown': function(e) {
								e = new Event(e).stop();
								currentList.mySlide.set(currentList.mySlide.step+scrollSpeed);
							}
				});	
	}
	currentList.wheeled=true
}

function setupSlider(obj,i,height){

	obj.addClass("liveSlider")
	
	if (i == null && height){
		areaName="sliderArea";
		knobName="sliderKnob";
		divHeight=height;
		steps=240;	
	}
	else if (height){
		divHeight=height-6;
		$('sliderTemplate').style.display="";
		createSlider = $('sliderTemplate').clone().injectTop(document.body);
		areaName=i+"Area";
		knobName=i+"Knob";
		createSlider.id = areaName
		$ES("div",createSlider.id)[0].id=knobName;				
		steps=parseInt(divHeight/2);
		
	}else{
		areaName="sliderArea";
		knobName="sliderKnob";
		divHeight=(height ? height : 433);
		steps=280;
	}
	scrollSpeed=10;
	$(areaName).setStyle("height",divHeight)
	obj.mySlide = new Slider($(areaName),$(knobName), {	
		steps: steps,	
		mode: 'vertical',	
		onChange: function(step){
			xIndex = parseInt(((obj.scrollHeight-divHeight)/steps)*step);
			obj.scrollTo(0,xIndex)
		}
	}).set(0);
	obj.slider=$(areaName);
	obj.slider.knob=$(knobName);
	obj.slider.parent=obj.slider.knob.parent=obj;
}
mySlider=[]
var navigation = function() {
    $$("div#tertiaryNav div").setStyle("display", "block");
    this.primaryNav = $("primaryNav");
    this.secondaryNav = $$("#primaryNav ul");
    this.primaryNavItems = $$("#primaryNav li");
    this.primaryNavChildren = primaryNav.getChildren();
    this.currentSection = $$("#primaryNav .current");
    this.tertiaryNav = $("tertiaryNav");
    var NAV_ORIGIN = "-999em"
    setupTertiaryNav();
    for (var i = 0; i < this.primaryNavChildren.length; i++) {
        var currentSection = this.currentSection;
        var currentChild = this.primaryNavChildren[i];
        var primaryNavItems = this.primaryNavItems;
        currentChild.onmouseover = function() {
            list = "cancelTimer"; //cancels the timer from firing
            changeClass(currentSection);
            clearClass(primaryNavItems)
            changeClass(this);
        }
        currentChild.onmouseout = function() {
            changeClass(this, 1000); //Set the menu hover delay here
        }
    }

    function clearClass(navItems) {
        for (var i = 0; i < navItems.length; i++) {
            navItems[i].removeClass("current");
        }
    }
    function changeClass(listItem, timer) {
        if (timer > 0) {//if a delay is set
            list = listItem; //assign the current listItem to a global variable	
            if (tymer) clearTimeout(tymer); //Stop the previous timer from firing
            tymer = setTimeout("hideMenu();", timer); //start the timer to hide the menu
        }
        else {
            listItem.toggleClass("current");
        }
    }
    hideMenu = function() {
        if (list != "cancelTimer" && list.hasClass("current")) {//If the timer hasn't been cancelled already and isn't already selected
            list.toggleClass("current"); //toggle & hide the navigation
            changeClass(currentSection); //change back to the default selection
        }
    }

    function changeClassWithDelay(listItem) {
        (function() { listItem.toggleClass("current") }).delay(1000);
    }

    function setupTertiaryNav() {

        if ($("tertiaryNav")) {
            $("tertiaryNav").style.width = "auto";
            $("tertiaryNav").style.overflow = "visible";
            //$$("div#tertiaryNav div").setStyle("visibility","visible")
        }
        setStyles();
        onNavClick();
        onNavItemClick();
    }

    function setStyles() {
        //return;
        var tertiaryNavArray = ($("tertiaryNav")) ? this.tertiaryNav.getChildren() : null;
        if (tertiaryNavArray) {
            // DEFINE MAX HEIGHT FOR DROPDOWNS (dynamic depending on size of browser window).
            maxDropHeight = parseInt(window.getSize().size.y / 2);
            minDropHeight = 200; // if list is under this height don't bother with bar no matter how big the browser window.
            tolerance = 50; // if list is within this many pixels of max drop height, don't bother with a scrollbar

            for (var i = 0; i < tertiaryNavArray.length; i++) {
                var currentNav = tertiaryNavArray[i];
                currentNav.addClass("tertiaryNavItem");
                var spanArray = $$(".tertiaryNavItem span");
                var listArray = $$(".tertiaryNavItem ul");

                for (var i = 0; i < spanArray.length; i++) {
                    var currentList = listArray[i];
                    var currentSpan = spanArray[i]
                    currentList.parentNav = currentNav
                    currentList.parentSpan = currentSpan
                    if (currentList.getSize().size.y >= (maxDropHeight + tolerance) && currentList.getSize().size.y > minDropHeight) {
                        currentList.setStyle("height", maxDropHeight + "px");
                        currentList.setStyle("overflow", "hidden");
                        setupSlider(currentList, i, maxDropHeight)
                        if (currentList.getSize().size.x > currentSpan.getSize().size.x) {
                            currentList.xWidth = currentList.getSize().size.x - 4
                        } else {
                            currentList.xWidth = currentSpan.getSize().size.x - 4
                        }
                        currentList.reposition = positionSlider
                        currentList.reposition()
                        currentList.slider.toggleClass("hide");
                    }
                    if (currentList.getSize().size.x <= currentSpan.getSize().size.x) {
                        currentList.setStyle("width", currentSpan.getSize().size.x + "px");
                    }
                }
            }
        }
        $('sliderTemplate').addClass("hide");
    }

    function onNavClick() {
        var tertiaryNavArray = $$(".tertiaryNavItem span a");
        var tertiaryListArray = $$(".tertiaryNavItem ul");
        for (var i = 0; i < tertiaryNavArray.length; i++) {
            var currentNav = tertiaryNavArray[i];
            var currentList = tertiaryListArray[i];
            currentList.readyToBlur = false;
            currentNav.siblingList = currentList;
            currentNav.onclick = function() {
                checkNav(this.siblingList);
                toggleNav(this.siblingList);
                this.focus()
                return false;
            }
            currentList.onmouseout = function() {
                this.readyToBlur = true;
            }
            currentList.onmouseover = function() {
                this.readyToBlur = false;
            }
            if (currentList.slider) {
                currentList.slider.onmouseout = function() {
                    this.parent.readyToBlur = true;
                }
                currentList.slider.onmouseover = function() {
                    this.parent.readyToBlur = false;
                }
                currentList.slider.knob.addEvent("blur", function() {
                    if (this.parent.readyToBlur && this.parent.hasClass("open")) {
                        checkNav(this.parent);
                        toggleNav(this.parent);
                    }
                    return false;
                })
            }
            currentNav.onmouseout = function() {
                this.siblingList.readyToBlur = true;
            }
            currentNav.onmouseover = function() {
                this.siblingList.readyToBlur = false;
            }
            currentNav.onblur = function() {
                if (this.siblingList.readyToBlur && this.siblingList.hasClass("open")) {
                    checkNav(this.siblingList);
                    toggleNav(this.siblingList);
                }
                return false;
            }

        }

        function checkNav(list) {
            var listItems = $$(".tertiaryNavItem ul");
            for (var i = 0; i < listItems.length; i++) {
                if (listItems[i].hasClass("open") && listItems[i] != list) {
                    listItems[i].setStyle("left", NAV_ORIGIN);
                    listItems[i].toggleClass("open");
                    if (listItems[i].slider) {
                        addWheeling(listItems[i])
                        listItems[i].slider.toggleClass("hide");
                        list.reposition();
                    }

                }
            }
        }

        function toggleNav(list) {
            var currentSpan = list.getPrevious();
            if (!list.hasClass("open")) {
                list.setStyle("top", currentSpan.getSize().size.y + "px");
                list.setStyle("left", currentSpan.offsetLeft + "px");
                list.toggleClass("open");
                if (list.slider) {
                    addWheeling(list)
                    list.slider.toggleClass("hide");
                    list.reposition();
                }

            } else {
                list.setStyle("left", NAV_ORIGIN);
                list.toggleClass("open");
                if (list.slider) {
                    addWheeling(list)
                    list.slider.toggleClass("hide");
                    list.reposition();
                }

            }
        }
    }

    function onNavItemClick() {
        var listItems = $$(".tertiaryNavItem ul li a");

        for (var i = 0; i < listItems.length; i++) {
            var currentItem = listItems[i];

            currentItem.originalonclick = currentItem.onclick;

            currentItem.onclick = function() {
                var itemHTML = this.innerHTML;
                var parentList = this.getParent().getParent();
                var parentSpan = parentList.getPrevious();
                setItem(itemHTML, parentSpan);
                closeList(parentList);

                if (!this.href && this.originalonclick)
                    return this.originalonclick();

                if (this.href && this.href != window.location.href) {
                    if (this.originalonclick) return this.originalonclick();

                    return true;
                }
                else
                    return false;
            }
        }

        function setItem(itemHTML, parentSpan) {
            parentSpan.getChildren().setHTML(itemHTML + '<img src="/img/decals/nav_arrow_ltbg.gif" />');
        }

        function closeList(list) {
            list.setStyle("left", NAV_ORIGIN);
            list.toggleClass("open");
            if (list.slider)
                list.slider.toggleClass("hide");
        }
    }
}

window.addEvent('domready', navigation);
window.addEvent('resize',repositionAllSliders)
window.addEvent('domready', fixMacFFxBug);


document.write('<div class="sliderAreas" style="display:none" id="sliderTemplate"><div class="sliderKnobs" id="knobTemplate"></div></div>');
