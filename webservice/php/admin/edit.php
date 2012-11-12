<?php 
require('functions.php'); 
authentication();
?>
<!DOCTYPE html>
<?php

if(isset($_POST["type"])) {
	$type = $_POST["type"];
	$id = $_POST["id"];
	
	$title = $_POST["title"];
	
	$date = $_POST["date"];
	$votecount = $_POST["votecount"];
	$author = $_POST["author"];
	
	if($type == "mot") {
		
		$tweetId = null;
		
		if(isset($_POST["tweetId"])) {
			$tweetId = $_POST["tweetId"];
		}
		
		$details1 = $_POST["details1"];
		$def1 = $_POST["def1"];
		$details2 = $_POST["details2"];
		$def2 = $_POST["def2"];
		$details3 = $_POST["details3"];
		$def3 = $_POST["def3"];
		
		if($id > 0) {
			editWord($id, $title, $date, $votecount, $author, $details1, $def1, $details2, $def2, $details3, $def3);
		}
		else {
			createWord($title, $date, $votecount, $author, $details1, $def1, $details2, $def2, $details3, $def3);
		}
		
		// Archive tweet
		if($tweetId != null) {
			archiveTweet($tweetId);
		}
	}
	else if($type == "contrepétrie") {
		
		$content = $_POST["content"];
		$solution = $_POST["solution"];
				
		if($id > 0) {
			editContrepeterie($id, $title, $date, $votecount, $author,$content,$solution);
		}
		else {
			createContrepeterie($title, $date, $votecount, $author,$content,$solution);
		}
	}
	gotoList();
}
else {

	if(isset($_GET["type"])) {
		$type = $_GET["type"];

		// Edit ?
		if(isset($_GET["id"])) {
			$id = $_GET["id"];
		}
		else {
			$id = -1;
		}
	}
	else {
		gotoList();
	}
}

function gotoList() {
	header( 'Location: list.php' ) ;
}
?>

<html lang=fr>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
<meta charset="utf-8">
<title>Culturez-Vous ! - Admin</title>
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<meta name="description" content="">
<meta name="author" content="">
<?php
includes();
?> 
</head>
<body>

<div class="navbar">
<div class="navbar-inner">
<div class="container-fluid"><a class="brand" href="index.php">Culturez-Vous!</a>
</div>
</div>
</div>

<div class="container-fluid">
<div class="row-fluid">
<div class="span3">
<div class="well sidebar-nav">
<ul class="nav nav-list">
	<li class="nav-header">Actions</li>
	<li><a href="index.php">Accueil</a></li>
	<li><a href="import.php">Import</a></li>
	<li><a href="list.php">Contenu</a></li>
	<li class="nav-header">Actions contenu</li>
	<li class="active"><a href="edit.php?type=<?php echo("$type");?>"><?php echo("$type");?></a></li>
</ul>
</div>
<!--/.well --></div>
<!--/span-->
<div class="span9">
<div class="hero-unit">
<h1>Culturez-Vous !</h1>
<p>Ajout de contenu</p>
</div>
</div>
<!--/span--></div>

<div class="span12"></div>

<form class="form-horizontal" method="post" action="edit.php">

<?php
$element = null;

if($id > 0) {
	if($type == "mot") {
		$element = getWord($id);
	}
	else if($type == "contrepétrie") {
		$element = getCTP($id);
	}
}

if($type == "mot") {

?>

<script>
$(document).ready(function () {
	$("#tweetDetail").hide();
});

function selectionChanged() {
	id = document.getElementById("tweets").value;
	selectTweet(id);
}

function selectTweet(id) {
	
	$.get("tweet.php?id=" + id, function (data) {
		$("#tweetDetail").show();
		document.getElementById("tweetContent").value = data;
	});
}

function selectRandom() {

	var select = document.getElementById('tweets');
	var items = select.getElementsByTagName('option');
	var index = Math.floor(Math.random() * items.length);

	select.selectedIndex = index;
	selectionChanged();
}

</script>

<div class="control-group">
	<label class="control-label">Pré-remplir:</label>
	<div class="controls">
		<select id="tweets" name="tweetId" onchange="selectionChanged()">
			<option></option>
		<?php
			$dbTweets = getTweetsForEdit();
			while ($donnees = mysql_fetch_array($dbTweets))
			{
				echo "<option value=\"".$donnees["id"]."\">".$donnees["title"]."</option>" ;
			}
		?>
		</select>
		
		<a href="#" class="btn btn-info" onclick=selectRandom()>Aléatoire</a>
	</div>
</div>

<div id="tweetDetail" class="control-group">
	<label class="control-label">Tweet :</label>
	<div class="controls">
		<textarea class="input-xlarge" id="tweetContent"	rows="4"></textarea>
	</div>
</div>
<?php }
?>

<div class="control-group"><label class="control-label">Titre</label>
<div class="controls">
<input type="text" class="input-xlarge"	name="title"
	value="<?php if($element != null) echo($element["title"]);?>" /></div>
</div>

<div class="control-group">

<script>
$(function() {
	$("#datepicker").datepicker({ dateFormat: 'yy-mm-dd' });
});
</script>

	<label class="control-label">Date</label>
	<div class="controls">

		<input type="text" class="input-xlarge"
			name="date"
			id="datepicker"
			value="<?php 
					if($element != null) echo($element["date"]); 
					else {
						if($type == "mot") {
							echo(getNextWordDate());
						}
						else if($type == "contrepétrie") {
							echo(getNextCTPDate());
						}
					}
				?>"/>
	
	</div>
</div>
<div class="control-group">
	<label class="control-label">Vote count</label>
	<div class="controls">
		<input type="text" class="input-xlarge"	name="votecount" value="<?php if($element != null) echo($element["voteCount"]); else echo(0); ?>"/>
	</div>
</div>

<?php	
	$authors = getAuthors()
?>
<div class="control-group"><label class="control-label">Auteur</label>
<div class="controls"><select name="author" multiple="multiple">

<?php
	while ($donnees = mysql_fetch_array($authors))
	{
		if(($element != null) && ($element["author_id"] == $donnees["author_id"])) {
			echo("<option selected=\"selected\">".$donnees["author_name"]."</option>");
		}
		else {
			echo("<option>".$donnees["author_name"]."</option>");
		}
	}

?>
</select></div>
</div>

<?php

if($type == "mot") {
	?>

<div class="control-group"><label class="control-label">Details
1</label>
<div class="controls"><textarea class="input-xlarge"
	name="details1" rows="2"><?php if($element != null) echo($element["details1"]);?></textarea></div>
</div>
<div class="control-group"><label class="control-label">Définition
1</label>
<div class="controls"><textarea class="input-xlarge" name="def1"
	rows="4"><?php if($element != null) echo($element["def1"]);?></textarea></div>
</div>
<div class="control-group"><label class="control-label">Details
2</label>
<div class="controls"><textarea class="input-xlarge"
	name="details2" rows="2"><?php if($element != null) echo($element["details2"]);?></textarea></div>
</div>
<div class="control-group"><label class="control-label">Définition
2</label>
<div class="controls"><textarea class="input-xlarge" name="def2"
	rows="4"><?php if($element != null) echo($element["def2"]);?></textarea></div>
</div>
<div class="control-group"><label class="control-label">Details
3</label>
<div class="controls"><textarea class="input-xlarge"
	name="details3" rows="2"><?php if($element != null) echo($element["details3"]);?></textarea></div>
</div>
<div class="control-group"><label class="control-label">Définition
3</label>
<div class="controls"><textarea class="input-xlarge" name="def3"
	rows="4"><?php if($element != null) echo($element["def3"]);?></textarea></div>
</div>

<?php
}
else if($type == "contrepétrie") {
	?>

<div class="control-group"><label class="control-label">Content</label>
<div class="controls"><textarea class="input-xlarge"
	name="content" rows="4"><?php if($element != null) echo($element["content"]);?></textarea></div>
</div>
<div class="control-group"><label class="control-label">Solution</label>
<div class="controls"><textarea class="input-xlarge"
	name="solution" rows="4"><?php if($element != null) echo($element["solution"]);?></textarea></div>
</div>

<?php
}
?> 
<input type="hidden" name="type"
	value="<?php echo("$type");?>"> <input type="hidden" name="id"
	value="<?php echo("$id");?>">

<div class="form-actions"><input type="submit"
	class="btn btn-primary" value="Enregistrer" /></div>

</form>

</div>
</body>

</html>