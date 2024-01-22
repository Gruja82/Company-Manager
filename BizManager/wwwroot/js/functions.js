function setSrc() {
    let img = document.getElementById("imgElement");
    const [inputValue] = document.getElementById("inputFileElement").files;
    if (inputValue) {
        img.src = URL.createObjectURL(inputValue);
    }
}