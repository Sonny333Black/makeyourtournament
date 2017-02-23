/**
 * Created by Sonny on 01.11.2016.
 */


// Looking for users
window.addEventListener('load',function() {
    var tournamentName= document.getElementById('turniername');
    var modus=document.getElementById('modus');
    normal();
    modus.oninput = function() {
        if(this.value==1){
            removeEventListenerAll();
            normal();
        }
        if(this.value==2){
            removeEventListenerAll();
            onlyGroups();
        }
        if(this.value==3){
            removeEventListenerAll();
            onlyKO();
        }
    };
    
    function normal() {
        creatInputAnzahlSpieler();
        creatInputTeamsProSpieler();
        setAnzahlTeamsForNormal();
        document.getElementById("anzahlGruppen").value=2;
        document.getElementById("anzahlSpieler").value=2;
        document.getElementById('teamsProSpieler').value=2;

        document.getElementById("showAnzahlGruppen").style.visibility="visible";

        document.getElementById("anzahlSpieler").addEventListener('change',setAnzahlTeamsForNormal);
        document.getElementById("anzahlGruppen").addEventListener('change',setAnzahlTeamsForNormal);

    }
    function setAnzahlTeamsForNormal() {
        var anzahlGruppen  =parseInt(document.getElementById("anzahlGruppen").value);
        var anzahlSpieler  =parseInt(document.getElementById("anzahlSpieler").value);
        var minManschaften = 0;

        switch(anzahlGruppen){
            case 2:
                if(anzahlSpieler <= 3){
                    minManschaften = 2;
                }else{
                    minManschaften = 1;
                }
                break;
            case 4:
                if(anzahlSpieler == 2){
                    minManschaften = 4;
                }else if(anzahlSpieler == 3){
                    minManschaften = 3;
                }else if(anzahlSpieler == 4){
                    minManschaften = 2;
                }else {
                    minManschaften = 1;
                }
                break;
            case 8:
                if(anzahlSpieler == 2){
                    minManschaften = 8;
                }else if(anzahlSpieler == 3){
                    minManschaften = 6;
                }else if(anzahlSpieler <= 5){
                    minManschaften = 4;
                }else if(anzahlSpieler <= 7){
                    minManschaften = 3;
                }else if(anzahlSpieler <= 15){
                    minManschaften = 2;
                }else{
                    minManschaften = 1;
                }
                break;
        }


        document.getElementById('teamsProSpieler').value=minManschaften;
        document.getElementById('teamsProSpieler').min=minManschaften;
    }
    function onlyGroups() {

        creatInputAnzahlSpieler();
        creatInputTeamsProSpieler();

        document.getElementById("showAnzahlGruppen").style.visibility="hidden";

        document.getElementById('anzahlGruppen').value=1;
        document.getElementById("anzahlSpieler").value=2;
        document.getElementById('teamsProSpieler').value=1;

        //document.getElementById('anzahlSpieler').addEventListener('change',giveRightTeamsAnzahlNormal);
    }

    function onlyKO(){
        var selectElement = document.createElement("select");
        selectElement.setAttribute("id", "anzahlSpieler");
        selectElement.setAttribute("name", "anzahlSpieler");

        var option2 = document.createElement("option");
        option2.setAttribute("value", 2);
        var t2 = document.createTextNode("2");
        option2.appendChild(t2);

        var option3 = document.createElement("option");
        option3.setAttribute("value", 4);
        var t3 = document.createTextNode("4");
        option3.appendChild(t3);

        var option4 = document.createElement("option");
        option4.setAttribute("value", 8);
        var t4 = document.createTextNode("8");
        option4.appendChild(t4);

        selectElement.appendChild(option2);
        selectElement.appendChild(option3);
        selectElement.appendChild(option4);

        document.getElementById("showAnzahlSpielerField").innerHTML="";
        document.getElementById("showAnzahlSpielerField").appendChild(selectElement);

        document.getElementById("anzahlGruppen").value=1;
        document.getElementById('teamsProSpieler').value=1;
        document.getElementById("showAnzahlGruppen").style.visibility="hidden";

        document.getElementById('anzahlSpieler').addEventListener('change',giveRightTeamsAnzahlOnlyKO);
        giveRightTeamsAnzahlOnlyKO();
    }

    function giveRightTeamsAnzahlOnlyKO() {

        var selectElement = document.createElement("select");
        selectElement.setAttribute("id", "teamsProSpieler");
        selectElement.setAttribute("name", "teamsProSpieler");

        if(document.getElementById('anzahlSpieler').value == 2){
            var option = document.createElement("option");
            option.setAttribute("value", 1);
            var t = document.createTextNode("1");
            option.appendChild(t);
            selectElement.appendChild(option);
            var option = document.createElement("option");
            option.setAttribute("value", 2);
            var t = document.createTextNode("2");
            option.appendChild(t);
            selectElement.appendChild(option);
            var option = document.createElement("option");
            option.setAttribute("value", 4);
            var t = document.createTextNode("4");
            option.appendChild(t);
            selectElement.appendChild(option);
            var option = document.createElement("option");
            option.setAttribute("value", 8);
            var t = document.createTextNode("8");
            option.appendChild(t);
            selectElement.appendChild(option);
        }
        if(document.getElementById('anzahlSpieler').value == 4){
            var option = document.createElement("option");
            option.setAttribute("value", 1);
            var t = document.createTextNode("1");
            option.appendChild(t);
            selectElement.appendChild(option);
            var option = document.createElement("option");
            option.setAttribute("value", 2);
            var t = document.createTextNode("2");
            option.appendChild(t);
            selectElement.appendChild(option);
            var option = document.createElement("option");
            option.setAttribute("value", 4);
            var t = document.createTextNode("4");
            option.appendChild(t);
            selectElement.appendChild(option);
        }
        if(document.getElementById('anzahlSpieler').value == 8){
            var option = document.createElement("option");
            option.setAttribute("value", 1);
            var t = document.createTextNode("1");
            option.appendChild(t);
            selectElement.appendChild(option);
            var option = document.createElement("option");
            option.setAttribute("value", 2);
            var t = document.createTextNode("2");
            option.appendChild(t);
            selectElement.appendChild(option);
        }

        document.getElementById("showTeamsProSpielerField").innerHTML="";
        document.getElementById("showTeamsProSpielerField").appendChild(selectElement);


    }


    function removeEventListenerAll() {
        document.getElementById('teamsProSpieler').value=1;
        document.getElementById('teamsProSpieler').min=1;
        document.getElementById('anzahlSpieler').removeEventListener('change',giveRightTeamsAnzahlOnlyKO);
        document.getElementById("anzahlSpieler").removeEventListener('change',setAnzahlTeamsForNormal);
        document.getElementById("anzahlGruppen").removeEventListener('change',setAnzahlTeamsForNormal);
    }
    


    function creatInputAnzahlSpieler() {
        var selectElement = document.createElement("input");
        selectElement.setAttribute("class", "form-control");
        selectElement.setAttribute("min", 2);
        selectElement.setAttribute("max", 32);
        selectElement.setAttribute("name", "anzahlSpieler");
        selectElement.setAttribute("type", "number");
        selectElement.setAttribute("value", 2);
        selectElement.setAttribute("id", "anzahlSpieler");
        document.getElementById("showAnzahlSpielerField").innerHTML="";
        document.getElementById("showAnzahlSpielerField").appendChild(selectElement);

    }
    function creatInputTeamsProSpieler() {

        var selectElement = document.createElement("input");
        selectElement.setAttribute("class", "form-control");
        selectElement.setAttribute("min", 1);
        selectElement.setAttribute("max", 16);
        selectElement.setAttribute("name", "teamsProSpieler");
        selectElement.setAttribute("type", "number");
        selectElement.setAttribute("value", 1);
        selectElement.setAttribute("id", "teamsProSpieler");
        document.getElementById("showTeamsProSpielerField").innerHTML="";
        document.getElementById("showTeamsProSpielerField").appendChild(selectElement);
    }
    


});