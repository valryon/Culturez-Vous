<?php 
require('functions.php'); 
authentication();
?>
<!DOCTYPE html>

<html lang=fr>
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
	<li><a href="index.php">Accueil</a></li>
	<li><a href="import.php">Import</a></li>
	<li class="active"><a href="list.php">Contenu</a></li>
</ul>
</div>
<!--/.well --></div>
<!--/span-->
<div class="span9">
<div class="hero-unit">
<h1>Culturez-Vous !</h1>
<p>Liste des éléments</p>
</div>
</div>
<!--/span--></div>

<div class="span12">

<p><a class="btn btn-primary" href="edit.php?type=mot">Ajouter un mot</a>
<a class="btn btn-primary" href="edit.php?type=contrepétrie">Ajouter une
contrepétrie</a></p>

<!--/row--> <!-- Liste des mots présents en base	-->
<table class="table table-striped table-bordered table-condensed">
	<thead>
		<tr>
			<th>Id</th>
			<th>Type</th>
			<th>Titre</th>
			<th>Date</th>
			<th>Favoris</th>
			<th>Auteur</th>
			<th colspan="2">Actions</th>
		</tr>
	</thead>
	<tbody>
	<?php

	$data = getElements();

	while ($donnees = mysql_fetch_array($data))
	{
		?>

		<tr>
			<td><?php echo($donnees["element_id"]); ?></td>
			<td><?php echo($donnees["type_name"]); ?></td>
			<td><?php echo($donnees["element_title"]); ?></td>
			
			<?php 
				$tempDate = strtotime( $donnees["element_date"] );
				$dateElement = date("d-m-Y", $tempDate);
			?>
			
			<td><?php echo($dateElement) ; ?></td>
			<td><?php echo($donnees["element_favoriteCount"]); ?></td>
			<td><?php echo($donnees["author_name"]); ?></td>
			<td><a
				href="edit.php?type=<?php echo($donnees["type_name"]); ?>&id=<?php echo($donnees["element_id"]); ?>"><i
				class="icon-edit"></i></a></td>
			<td><a
				onclick="<?php echo("if(confirm('Supprimer cet élément (".$donnees['element_title'].") ?')) window.location = 'delete.php?id=".$donnees['element_id']."';"); ?>"
				href="#" ?><i
				class="icon-remove"></i></a></td>
		</tr>

		<?php
	}
	?>
	</tbody>
</table>
</div>
</div>
<hr>

<footer>
<p>© Valryon 2012</p>
</footer>

<!--/.fluid-container-->
</body>
<?php
includes();
?> 
</html>