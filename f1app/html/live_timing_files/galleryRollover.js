/*
    Display a transparent png over the gallery thumbnail on rollover.
*/

function galleryRollover() {
    var galleryLinks = $$(".gallery a");

    for(i=0;i<galleryLinks.length;i++){
        var currentLink = galleryLinks[i];
        
        createOverlay(currentLink);
        
        currentLink.onmouseover = function(){
            addOverlay(this);
        }
        
        currentLink.onmouseout = function(){
            removeOverlay(this);
        }
    }
}

function createOverlay(link){
    new Element('span').setProperty('class', 'galleryOverlay').injectInside(link);
}

function addOverlay(link){
    var overlay = link.lastChild;
    overlay.style.display = "block";
    overlay.setOpacity(0.4);
    overlay.style.width = link.getSize().size.x + "px";
    overlay.style.height = link.getSize().size.y + "px";
}

function removeOverlay(link){
    var overlay = link.lastChild;		
    overlay.style.display = "none"
}

window.addEvent('domready', galleryRollover);
