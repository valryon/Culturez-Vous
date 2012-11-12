<?php 
require('functions.php'); 
authentication();

if(isset($_GET["id"])) {
	$id = $_GET["id"];

	delete($id);
}

header( 'Location: list.php' ) ;

?>