<?php 
require('functions.php'); 
authentication();
?>
<!DOCTYPE html>

<html lang="fr">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
<meta charset="utf-8">
<title>Culturez-Vous ! - Admin</title>
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<meta name="robots" content="noindex, no follow">
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
	<li class="active"><a href="#">Accueil</a></li>
	<li><a href="import.php">Import</a></li>
	<li><a href="list.php">Contenu</a></li>
</ul>
</div>
<!--/.well --></div>
<!--/span-->
<div class="span9">
<div class="hero-unit">
<h1>Culturez-Vous !</h1>
<p>Admin</p>
</div>
<div class="span9">
	<div class="alert alert-info">
		  Back-Office !
	</div>
</div>
</div>
<!--/span--></div>
<!--/row-->

<hr>

<footer>
<p>© Valryon 2012</p>
</footer></div>
<!--/.fluid-container-->