<?php 
require('functions.php'); 
authentication();

if(isset($_GET["id"]))
{
	$id = $_GET["id"];
	
	echo getTweet($id);
}
else {
	echo "Pas de données";
}
?>