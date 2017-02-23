/**
 * Created by Sonny on 01.11.2016.
 */


// Looking for users
window.addEventListener('load',function() {
    var userFields = document.getElementsByClassName("userFields");
    for(var i = 0 ; i < userFields.length ; i++){
        var userfield = document.getElementById(i+"user");
        userfield.oninput = function() {
            if(this.value != 0){
                showCheckButtonAndDisablePickTeams();
                $.get('searchUserPick',  {user_id:this.value,position:this.id}, onSuccess);
            }else{
                showCheckButtonAndDisablePickTeams();
                noUserSelected(this.id);
            }
        };
    }

    function noUserSelected(id){
        id = id.slice(0, -4);
        var tempElement = document.getElementById(id+'teamField');
        tempElement.innerHTML ="";
        console.log(tempElement);
    }

    function onSuccess(data) {
        var action = data.action;
        switch (action) {
            case "teamsFromUser":
                var userFields = document.getElementsByClassName("userFields");
                for(var i = 0 ; i < userFields.length ; i++){
                    var userfield = document.getElementById(i+"user");
                    if(data.position==userfield.id){

                        var teamField = document.getElementById(i+"teamField");
                        teamField.innerHTML="";
                        for(var j = 0 ; j < document.getElementById("countTeamsPerUser").value ; j++) {
                            var selectElement = document.createElement("select");
                            selectElement.setAttribute("class", "form-control selectFieldPerTeams");
                            selectElement.setAttribute("name", i+'team'+j);
                            for (var k = 0; k < data.teams.length; k++) {
                                var optionElement = document.createElement("option");
                                optionElement.setAttribute("value", data.teams[k].id);
                                var t = document.createTextNode(data.teams[k].name);
                                optionElement.appendChild(t);
                                selectElement.appendChild(optionElement);
                                selectElement.oninput = function() {
                                    showCheckButtonAndDisablePickTeams();
                                }
                            }
                            teamField.appendChild(selectElement);
                            var br = document.createElement("br");
                            teamField.appendChild(br);
                        }
                    }
                }
                break;
            default:
                return;
        }
    }
    document.getElementById("checkButton").addEventListener("click",checkAll);
    function showCheckButtonAndDisablePickTeams(){
        document.getElementById("checkButton").style = "display:block";
        document.getElementById("pickTeams").style = "display:none";
    }
    function checkAll() {
        if(checkUser()){
            if(checkTeams()){
                var flash=document.getElementById("info-flash");
                flash.innerHTML="";
                document.getElementById("checkButton").style = "display:none";
                document.getElementById("pickTeams").style = "display:block";

            }else {
                var flash=document.getElementById("info-flash");
                flash.innerHTML="";
                var t = document.createTextNode("Keine doppelten Teams auswählen. Sollten Sie zu wenig haben erstellen Sie mehr.");
                flash.appendChild(t);
            }
        }else {
            var flash=document.getElementById("info-flash");
            flash.innerHTML="";
            var t = document.createTextNode("User darf nicht doppelt drin sein und alles muss ausgefüllt sein.");
            flash.appendChild(t);
        }

    }
    function checkUser(){
        var userCheck = true;
        var userValueArray  =new Array;
        var userFields = document.getElementsByClassName("userFields");
        for(var i = 0 ; i < userFields.length ; i++){
            var select = document.getElementById((i+"user"));
            userValueArray[i] = select.value;
        }

        var sorted_arr = userValueArray.slice().sort();

        for (var i = 0; i < sorted_arr.length - 1; i++) {
            if (sorted_arr[i + 1] == sorted_arr[i] || sorted_arr[i]==0 || 0==sorted_arr[i + 1]) {
                userCheck=false;
            }
        }
        return userCheck;
    }
    function checkTeams() {
        var teamValueArray= new Array() ;
        var teamCheck = true;
        var count = 0;
        var userFields = document.getElementsByClassName("userFields");
        for(var j = 0 ; j < userFields.length ; j++){
                var select = document.getElementById((j+"teamField"));

                var allFieldsSelectPerTeams = select.getElementsByClassName("selectFieldPerTeams");

                for (var k = 0; k < allFieldsSelectPerTeams.length; k++) {
                    teamValueArray[count] = allFieldsSelectPerTeams[k].value;
                    count++;
                }
            }

            var sorted_arr = teamValueArray.slice().sort();
            for (var i = 0; i < sorted_arr.length - 1; i++) {
                if (sorted_arr[i + 1] == sorted_arr[i]) {
                    teamCheck=false;
                }
            }
            return teamCheck;
        }



});