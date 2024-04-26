// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function pass() {
    return
}
class DOM {
    constructor() { }

    static div(parent, id, className) {
        const node = document.createElement("div");
        const parentNode = document.getElementById(parent)

        id ? node.id = id : pass()
        className ? node.classList.add(className) : pass()

        parentNode.appendChild(node)
    }

    static p(parent, text, id, className) {
        const node = document.createElement("p");
        const parentNode = document.getElementById(parent)

        id ? node.id = id : pass()
        className ? node.classList.add(className) : pass()
        node.innerText = text;

        parentNode.appendChild(node)
    }

    static img(parent, src, alt = "", id, className) {
        const node = document.createElement("img");
        const parentNode = document.getElementById(parent)

        node.src = src;
        node.alt = alt;
        id ? node.id = id : pass()
        className ? node.classList.add(className) : pass()

        parentNode.appendChild(node)
    }

    static removeChildren(parent) {
        const parentNode = document.getElementById(parent)
        while (parentNode.firstChild) {
            parentNode.firstChild.remove()
        }
    }

    static toggleClassMouseover(target, className) {
        target.addEventListener("mouseover", function () {
            target.classList.toggle(className)
        })
        target.addEventListener("mouseout", function () {
            target.classList.toggle(className)
        })
    }

    static toggleClassTargetMouseover(elem1, elem2, className) {
        elem1.addEventListener("mouseover", function () {
            elem2.classList.toggle(className)
        })
        elem1.addEventListener("mouseout", function () {
            elem2.classList.toggle(className)
        })

    }

    static createNotificationItem(parentId) {
        const elemContainer = document.createElement("div")

        const imgContainer = document.createElement("div")
        const image = document.createElement("img")
        imgContainer.appendChild(image)
        elemContainer.appendChild(imgContainer)

        const notifContainer = document.createElement("div")
        const text = document.createElement("p")
        notifContainer.appendChild(text)
        elemContainer.appendChild(notifContainer)

        const controlContainer = document.createElement("div")
        const acceptButton = document.createElement("button")
        const rejectButton = document.createElement("button")
        controlContainer.appendChild(acceptButton)
        controlContainer.appendChild(rejectButton)
        notifContainer.appendChild(controlContainer)

        const parent = document.getElementById(parentId)
        parent.appendChild(elemContainer)
    }

}


