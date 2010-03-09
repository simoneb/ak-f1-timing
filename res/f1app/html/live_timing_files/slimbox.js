/*
	Slimbox v1.31 - The ultimate lightweight Lightbox clone
	by Christophe Beyls (http://www.digitalia.be) - MIT-style license.
	Inspired by the original Lightbox v2 by Lokesh Dhakar.
*/

var MODE = "DEFAULT"
var bBuyImage = false;
var copyHTML="", footerHTML="", footerBuyHTML="", closeHTML="", nextButtonHTML="", backButtonHTML="", slideButtonHTML="", buyButtonHTML="", buyBackButtonHTML=""
var currentIndex = 0;

var tvOriginalImageSize = "597x478";
var tvTargetImageSize = "225x";
var buyImageTeaser = 'Buy Image';

function LoadButtons(){
    copyHTML="";
    closeHTML       = "<div class='slimClose first'><a href='javascript:void(0);'>close</a></div>";

    nextButtonHTML  = "<div class='footBox next'><a href='javascript:void(0);'>next</a></div>";
    backButtonHTML  = "<div class='footBox back'><a href='javascript:void(0);'>back</a></div>";
    slideButtonHTML = "<div class='footBox slide " + (bBuyImage ? "" : "first") + "'><a href='javascript:void(0);'>start slideshow</a></div>";
    buyButtonHTML   = "<div class='footBox buy first' style='visibility:" + (bBuyImage ? 'visible;' : 'hidden;') + "'><a href='javascript:void(0);'>send to my mobile</a></div>";
    buyBackButtonHTML  = "<div class='footBox buyBack first' style='visibility:" + (bBuyImage ? 'visible;' : 'hidden;') + "'><a href='javascript:void(0);'>back</a></div>";

    footerHTML  = nextButtonHTML + backButtonHTML + slideButtonHTML + buyButtonHTML + "</div>";
    footerBuyHTML = buyBackButtonHTML + "</div>";
}

function buyImageResponse(sMessage){
    lightbox=Lightbox;
    lightbox.buyForm.setStyle('display', 'none');
    lightbox.buyCaption.setHTML("<p>" + sMessage + "</p>");
   
    var nextEffect = lightbox.nextEffect.bind(this);		            
    this.fxx = {
        resize: lightbox.center.effects({duration: lightbox.options.resizeDuration, transition: lightbox.options.resizeTransition, onComplete: nextEffect})
    };   
    this.fxx.resize.start({height: lightbox.buy.clientHeight + lightbox.topContainer.clientHeight - 5})
}

function initNewButtons(){
	nextButton=$$("div.next a")[0];
	prevButton=$$("div.back a")[0];
	slideButton=$$("div.slide a")[0];
	buyButton=$$("div.buy a")[0];
	buyBackButton=$$("div.buyBack a")[0];
	nextButton.lightbox=prevButton.lightbox=slideButton.lightbox=buyButton.lightbox=buyBackButton.lightbox=Lightbox;
	Lightbox.slideButton=slideButton;
	
	nextButton.onclick=function(){
        this.lightbox.slideButton.switchOff();	    
	    
	    if (currentIndex == Lightbox.anchors.length-1){
		    this.lightbox.changeImage(0);
	    }else{
		    this.lightbox.next()	    
	    }
	}
	prevButton.onclick=function(){
	    this.lightbox.slideButton.switchOff();
	    this.lightbox.stopSlide();
	    
	    if (currentIndex == 0){
            this.lightbox.changeImage(Lightbox.anchors.length-1);   
	    }else{
		    this.lightbox.previous();
	    }		
	}
	buyBackButton.onclick=function(){
	    MODE = "DEFAULT";
	    this.lightbox.closeBuy();
	    this.lightbox.buy.setStyle('display', 'none');

	    this.lightbox.changeImage(currentIndex);
	    $("lbTop").setStyle('width', '617px');
	}
	buyButton.onclick=function(){
	    if (bBuyImage) {
	        MODE = "BUY";
	        
            this.lightbox.center.setStyle('background', 'none');
            this.lightbox.image.setStyle('display', 'none');
            this.lightbox.bottomContainer.setStyle('display', 'none');
            
            //Populate Elements
            currentImagePath = this.lightbox.images[currentIndex][0];
            this.lightbox.buyImage.setProperties({'src': currentImagePath.replace(tvOriginalImageSize, tvTargetImageSize)});
            this.lightbox.buyCaption.setHTML(buyImageTeaser);
                	
        	$("lbTop").setStyle('width', '245px');
            this.lightbox.buy.setStyles({'display': ''});
            this.lightbox.buyForm.setStyle('display', '');
        	
        	this.lightbox.center.setStyles({height:  this.lightbox.buy.clientHeight + this.lightbox.topContainer.clientHeight + 'px', width: '245px', marginLeft: '-130px', display: ''})

        	this.lightbox.topContainer.setStyles({marginLeft: '-130px', display: ''})
            
            initOverLabels();
		}else{
		    buyImageResponse("This image is not for sale.");
		}
		return false;
	}
	
	if(Lightbox.anchors.length>1){
		slideButton.switchOn=function(){
			this.lightbox.startSlide();
			this.innerHTML="Stop slideshow"
			this.oldClick=this.onclick;
		}
		slideButton.switchOff=function(){
			this.lightbox.stopSlide();
			this.innerHTML="Start slideshow"
			if(this.oldClick)
			this.onclick=this.oldClick;
		}
		slideButton.onclick=function(){
			this.switchOn()
			this.onclick=function(){
				this.switchOff()
			}
		}		
	}else{
		
	}
}

var Lightbox = {
	init: function(options){
		LoadButtons();
		this.sliding=false;
		this.options = Object.extend({
			resizeDuration: 0,
			resizeTransition: 0,
			initialWidth: 0,
			initialHeight: 0,
			animateCaption: false
		}, options || {});
		
		this.anchors = [];
		$each(document.links, function(el){
			if (el.rel && el.rel.test(/^lightbox/i)){
				el.onclick = this.click.pass(el, this);
				el.oncontextmenu = function(){ RightClickImage(); return false; };
				this.anchors.push(el);
			}
		}, this);
		this.eventKeyDown = this.keyboardListener.bindAsEventListener(this);
		this.eventPosition = this.position.bind(this);
		this.overlay = new Element('div').setProperties({id: 'lbOverlay'}).injectInside(document.body);
		this.center = new Element('div').setProperties({id: 'lbCenter'}).setStyles({width: this.options.initialWidth+'px', height: this.options.initialHeight+'px', marginLeft: '-'+(this.options.initialWidth/2)+'px', display: 'none'}).injectInside(document.body);
		this.topContainer = new Element('div').setProperty('id', 'lbTopContainer').setStyle('display', 'none').injectInside(document.body);
		this.topInner = new Element('div').setProperty('id', 'lbTop').injectInside(this.topContainer);
		new Element('div').setProperties({id: 'lbCloseLinkTop'}).injectInside(this.topInner).onclick = this.overlay.onclick = this.close.bind(this);
		
		new Element('div').setStyle('clear', 'both').injectInside(this.topInner);
		
		this.image = new Element('div').setProperties({id: 'lbImage'}).injectInside(this.center);
		this.image.addEvent('contextmenu', function() { RightClickImage(); return false; });
		
		this.prevLink = new Element('a').setProperties({id: 'lbPrevLink', href: '#'}).setStyle('display', 'none').injectInside(this.image);
		this.nextLink = this.prevLink.clone().setProperty('id', 'lbNextLink').injectInside(this.image);
		this.buyLink = this.prevLink.clone().setProperty('id', 'lbBuyLink').injectInside(this.image);
		
		this.bottomContainer = new Element('div').setProperty('id', 'lbBottomContainer').setStyle('display', 'none').injectInside(document.body);
		this.bottom = new Element('div').setProperty('id', 'lbBottom').injectInside(this.bottomContainer);
		this.number = new Element('div').setProperty('id', 'lbNumber').injectInside(this.bottom);
		this.caption = new Element('div').setProperty('id', 'lbCaption').injectInside(this.bottom);
		this.copy = new Element('div').setProperty('id', 'lbCopy').setHTML(copyHTML).injectInside(this.bottom);
		this.foot = new Element('div').setProperty('id', 'lbFoot').injectInside(this.bottom);
		
		new Element('div').setStyle('clear', 'both').injectInside(this.bottom);
		this.foot.setHTML(footerHTML)
		
		$("lbCloseLinkTop").setHTML(closeHTML)
		var nextEffect = this.nextEffect.bind(this);
		this.fx = {
			overlay: this.overlay.effect('opacity', {duration: 500}).hide(),
			resize: this.center.effects({duration: this.options.resizeDuration, transition: this.options.resizeTransition, onComplete: nextEffect}),
			topInner: this.topInner.effect('opacity', {duration: 500, onComplete: nextEffect}),
			image: this.image.effect('opacity', {duration: 500, onComplete: nextEffect}),
			bottom: this.bottom.effect('margin-top', {duration: 400, onComplete: nextEffect}),
			resizeTop: this.center.effects({duration: this.options.resizeDuration, transition: this.options.resizeTransition, onComplete: nextEffect}),
			resizeBottom: this.center.effects({duration: this.options.resizeDuration, transition: this.options.resizeTransition, onComplete: nextEffect})
		};

		this.preloadPrev = new Image();
		this.preloadNext = new Image();
		
        this.buy = new Element('div').setProperties({id: 'lbBuyImage'}).setStyles({width: '245px'}).injectInside(this.center);
        this.buyContainer = new Element('div').setProperties({id: 'buyImageWrapper'}).injectInside(this.buy);	    
        this.buyContainer.className = 'secondaryArticleContent';
        
        this.buyImage = new Element('img').setProperties({id: 'loginIMG'}).injectInside(this.buyContainer);
        this.buyImage.addEvent('contextmenu', function() { RightClickImage(); return false; });
        
        this.buyCaption = new Element('div').injectInside(this.buyContainer);    	
        this.buyCaption.className = "buyCaption";
        this.buyForm = new Element('div').injectInside(this.buyContainer);	    
        
        this.foot = new Element('div').setProperty('id', 'lbFoot').injectInside(this.buyContainer);
	    this.foot.setHTML(footerBuyHTML);
        
        buyHTML='<h3>LOGIN</h3><table summary="Login for Buy Image" cellpadding="0" cellspacing="5">' +
            '<tr><td><div class="fieldpairing"><label for="loginEmail" class="overlabel">Username</label><input class="grey required" id="loginEmail" name="email" type="text" value="" /></div></td></tr>' +
            '<tr><td><div class="fieldpairing"><label for="loginPassword" class="overlabel">Password</label><input class="actionBox required" id="loginPassword" name="password" type="password" value="" /><input id="Login" class="required action" type="image" src="/img/decals/button_search.gif" value="login" alt="login" onclick="buyImageResponse(processImageDownload(\'' + tvTargetImageSize + '\', this));" /></div></td></tr>' +
        '</table>';        
        this.buyForm.setHTML(buyHTML);
	},

	click: function(link){
		if (link.rel.length == 8) return this.show(link.href, link.title);
		var j, imageNum, images = [];
		this.anchors.each(function(el){
			if (el.rel == link.rel){
				
				for (j = 0; j < images.length; j++) if(images[j][0] == el.href) break;
				if (j == images.length){
					images.push([el.href, el.title, el.rev]);
					if (el.href == link.href) imageNum = j;
				}
			}
		}, this);
		return this.open(images, imageNum);
	},

	show: function(url, title){
		LoadButtons();
		return this.open([[url, title]], 0);
	},

	open: function(images, imageNum){
		this.images = images;
		this.position();
		this.setup(true);
		this.top = window.getScrollTop() + (window.getHeight() / 15);
		this.center.setStyles({top: this.top+'px', display: ''});
		this.fx.overlay.start(0.8);
		return this.changeImage(imageNum);
    },

	position: function(){
		this.overlay.setStyles({top: window.getScrollTop()+'px', height: window.getHeight()+'px', display: 'block'}); 
	},

	setup: function(open){
		this.showHideFlash(open);
		var fn = open ? 'addEvent' : 'removeEvent';
		window[fn]('scroll', this.eventPosition)[fn]('resize', this.eventPosition);
		document[fn]('keydown', this.eventKeyDown);
		this.step = 0;
		if (!(open)) firefoxForceRerender();
	},

	showHideFlash: function(open) {
		
		var elements = $A(document.getElementsByTagName('object'));
		elements.extend(document.getElementsByTagName(window.ie ? 'select' : 'embed'));
		elements.each(function(el){
				if (open)
				{
					 el.lbBackupStyle = (el.style.visibility)?(el.style.visibility+''):'';
					 if (el.lbBackupStyle=='') el.lbBackupStyle='visible';
				}
				
				el.style.visibility = open ? 'hidden' : el.lbBackupStyle;
		});
		
		if (!(open)) this.overlay.setStyles({top: '0px', height: '0px', display: 'none'});
	},
	
	keyboardListener: function(event){
		if (MODE!="BUY"){
		switch (event.keyCode){
			case 27: case 88: case 67: this.close(); break;
			case 37: case 80: this.previous(); break;	
			case 39: case 78: this.next();
		}
		}
	},

	previous: function(){
		return this.changeImage_NextPrev(this.activeImage-1);
	},

	next: function(){
		return this.changeImage_NextPrev(this.activeImage+1);
	},
	reposition: function(){
		this.topContainer.setStyles({top: (window.getScrollTop() + (window.getHeight() / 15))+'px', marginLeft: this.center.style.marginLeft, display: ''});
//		this.bottomContainer.setStyles({top: (window.getScrollTop() + (window.getHeight() / 15) + this.center.clientHeight - 37 + this.topContainer.clientHeight)+'px', height: '0px', marginLeft: this.center.style.marginLeft, display: ''});
// above line previous version;
		this.bottomContainer.setStyles({top: (window.getScrollTop() + (window.getHeight() / 15) + this.center.clientHeight - 37 + this.topContainer.clientHeight)+'px', marginLeft: this.center.style.marginLeft, display: ''});
// above line test only;

		this.center.setStyles({top: (window.getScrollTop() + (window.getHeight() / 15))+'px', marginLeft: this.center.style.marginLeft, display: ''});
	},
	changeImage: function(imageNum){
		if (this.step || (imageNum < 0) || (imageNum >= this.images.length)) return false;
		this.step = 1;
		this.activeImage = imageNum;
		this.center.style.backgroundColor = '';
		this.bottomContainer.style.display = this.topContainer.style.display = 'none';		
		this.fx.image.hide();
		this.center.className = 'lbLoading';
		this.preload = new Image();
		this.preload.onload = this.nextEffect.bind(this);
		this.preload.src = this.images[imageNum][0];
		currentIndex = imageNum;
		return false;
	},

	// next/prev change - change without transition;
	changeImage_NextPrev: function(imageNum) {
		if (this.step || (imageNum < 0) || (imageNum >= this.images.length)) return false;
		this.step = 1;
		this.activeImage = imageNum;
		this.preload = new Image();
		this.preload.onload = this.slideShowNextEffect.bind(this);
		this.preload.src = this.images[imageNum][0];
		currentIndex = imageNum;
		return false;
	},
	
	// used on slide show only;
	// next/previous display for slideshow without any transitional effects;
	slideShowNextEffect: function() {
		
		// step 1 effects;
		this.center.className = '';
		this.center.height = this.center.clientHeight + this.topContainer.clientHeight+'px';
		this.topContainer.style.height = '';
		this.topInner.style.width = this.preload.width + 20 + 'px';
		this.image.style.backgroundImage = 'url('+this.images[this.activeImage][0]+')';
		this.image.style.width = this.bottom.style.width = this.preload.width+'px';
		this.image.style.height = this.preload.height+'px';
		
		this.caption.setHTML(this.images[this.activeImage][1] || '');
		this.number.setHTML((this.images.length == 1) ? '' : '('+(this.activeImage+1)+' / '+this.images.length+') -&nbsp;');
		this.copy.setHTML(this.images[this.activeImage][2] || '');
		
		if (this.activeImage) this.preloadPrev.src = this.images[this.activeImage-1][0];
		if (this.activeImage != (this.images.length - 1)) this.preloadNext.src = this.images[this.activeImage+1][0];
		
		this.center.style.height = this.image.offsetHeight + 'px';
		

		this.center.style.width = this.image.offsetWidth + 'px';
		this.center.style.marginLeft = (-this.image.offsetWidth/2) + 'px';

		this.reposition();

		this.center.style.backgroundColor = '#000';
		this.bottomContainer.style.height = '';

		this.step = 0;
	},
	
	nextEffect: function(){	
		switch (this.step++){
		case 1:
			if(this.images.length<2){
				$$(".footBox").setStyle("display","none")
			}	
			this.center.className = '';
			this.center.height = this.center.clientHeight + this.topContainer.clientHeight+'px';
			this.topContainer.style.height = '';
			this.topInner.style.width = this.preload.width + 20 + 'px';
			this.image.style.backgroundImage = 'url('+this.images[this.activeImage][0]+')';
			this.image.style.width = this.bottom.style.width = this.preload.width+'px';
			this.image.style.height = this.preload.height+'px';
			this.caption.setHTML(this.images[this.activeImage][1] || '');
			this.number.setHTML((this.images.length == 1) ? '' : '('+(this.activeImage+1)+' / '+this.images.length+') -&nbsp;');
			this.copy.setHTML(this.images[this.activeImage][2] || '');
			if (this.activeImage) this.preloadPrev.src = this.images[this.activeImage-1][0];
			if (this.activeImage != (this.images.length - 1)) this.preloadNext.src = this.images[this.activeImage+1][0];
			if (this.center.clientHeight != this.image.offsetHeight){
				this.fx.resize.start({height: this.image.offsetHeight});
				break;
			}
			this.step++;
		case 2:
			if (this.center.clientWidth != this.image.offsetWidth){
				this.fx.resize.start({width: this.image.offsetWidth, marginLeft: -this.image.offsetWidth/2});
				break;
			}
			this.step++;
		case 3:
			this.reposition();
			this.fx.image.start(1);
			break;
		case 4:
			this.center.style.backgroundColor = '#000';
			if (this.options.animateCaption){
				this.fx.bottom.set(-this.bottom.offsetHeight);
				this.bottomContainer.style.height = '';
				this.fx.bottom.start(0);
				break;
			}
			this.bottomContainer.style.height = '';
		case 5:
			this.step = 0;
		}
	},

	closeBuy: function(){
		this.image.style.display='';
		$("lbTop").setStyle('width', 'auto');
	},

	close: function(){
		this.closeBuy();
			
		if(this.sliding==true){
			this.slideButton.switchOff();
		}
		if (this.step < 0) return;
		this.step = -1;
		if (this.preload){
			this.preload.onload = Class.empty;
			this.preload = null;
		}
		for (var f in this.fx) this.fx[f].stop();
		this.center.style.display = this.topContainer.style.display = this.bottomContainer.style.display = 'none';
		this.fx.overlay.chain(this.setup.pass(false, this)).start(0);
		
        //firefoxForceRerender();			
		
		return false;
	},
	
	slide: function(){
		if ((this.activeImage < this.images.length) && this.sliding!=false){
			this.next();
			(function(){Lightbox.slide()}).delay(5000);	
		}
		if(this.activeImage == (this.images.length-1)){
			//this.slideButton.switchOff();
			this.changeImage(0);
		}
	},
	
	scroll: function(){
	//	this.step=3;
	//	this.nextEffect();
	},
	
	startSlide: function(){
		this.sliding=true;	
		this.slide();
	},
	
	stopSlide: function(){
		this.sliding=false;	
		return false;
	}
	
};

window.addEvent('domready', Lightbox.init.bind(Lightbox));
window.addEvent('domready', initNewButtons);
window.addEvent('scroll', Lightbox.scroll.bind(Lightbox));
