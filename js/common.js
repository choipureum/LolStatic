
function Onsubmit() {
    if ($("#search_summoner").val() == "" || $("#search_summoner").val() == null) {
        alert("소환사명을 입력해주세요");
        return false;
    }
    else {
        $("#LolForm").submit();
    }
}

