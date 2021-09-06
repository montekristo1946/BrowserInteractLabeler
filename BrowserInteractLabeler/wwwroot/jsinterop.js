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