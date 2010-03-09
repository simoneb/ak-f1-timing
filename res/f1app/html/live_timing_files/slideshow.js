// Slideshow script by Dominic Hoare; adapted by Toby Travis 14/11/07 to seperate caption from image behaviour

var Slideshow = new Class({
	initialize: function(className,timeDelay){
		if($$(className).length<1) return;
		this.delay=timeDelay;
		
		this.imageSelector="div" + className + " img";
		this.imageArray=$$(this.imageSelector);
		this.imageArray.setStyle("visibility","hidden");
		
		this.captionSelector="div" + className + " table";
		this.captionArray=$$(this.captionSelector);
		this.captionArray.setStyle("visibility","hidden");

		this.linkSelector="div" + className + " a";
		this.linkArray=$$(this.captionSelector);
		this.linkArray.setStyle("visibility","hidden");
		
		this.finalImageIndex=this.imageArray.length-1;
		this.finalImage=this.imageArray[this.finalImageIndex];
		
		this.count=this.finalImageIndex;
		this.rotate();
	},
	nextImage: function(){
		this.imageArray.setStyle("z-index",2);
		this.imageArray[this.count].setStyle("z-index",3);
		this.transitionDuration=500;
		if(window.gecko && (navigator.appVersion.indexOf("Mac")!=-1)){this.transitionDuration=0;}
		var myFx=new Fx.Style(this.imageArray[this.count],'opacity',{duration:this.transitionDuration});
		myFx.start(0,1).chain(function(){
			this.lastImage.setStyle("visibility","hidden");
		}.bind(this));
	},
	nextCaption: function(){
		this.captionArray.setStyle("visibility","hidden");
		this.captionArray[this.count].setStyle("visibility","visible");
		
		this.linkArray.setStyle("visibility","hidden");
		this.linkArray[this.count].setStyle("visibility","visible");
	},
	rotate: function(){
		this.lastImage=this.imageArray[this.count];
		this.count++;
		if (this.count>this.finalImageIndex){
		 	this.count=0;
		 	this.lastImage=this.finalImage;
		}
		this.nextImage();
		this.nextCaption();
		this.rotate.delay(this.delay,this);
	}
});

function createSlideShows(){
	var domSlide = new Slideshow(".imgContainer",5000);
}

window.addEvent("domready",createSlideShows);