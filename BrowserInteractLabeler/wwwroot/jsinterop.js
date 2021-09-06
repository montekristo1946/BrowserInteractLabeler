function getDivCanvasOffsets(el) {
    var obj = {};
    obj.offsetLeft = el.offsetLeft;
    obj.offsetTop = el.offsetTop;
    obj.clientRect= el.getBoundingClientRect();
    return JSON.stringify(obj);
}

function SendAlert(text) {
    alert(`Alert:${text}!`);
}


function FocusElement (id){
    const element = document.getElementById(id);
    element.focus();
}

function ClickElement (id){
    const element = document.getElementById(id);
    element.click();
}

function GetSizeElement (id){
    console.log("Run GetSizeElement",id)

    const element = document.getElementById(id);
    if(element == null)
    {
        console.log("Run GetSizeElement null",id)
        return "";
    }
    console.log("Run GetSizeElement element:",element)
    const obj = {};
    obj.offsetHeight = element.offsetHeight;
    obj.offsetWidth = element.offsetWidth;
    return JSON.stringify(obj);
}