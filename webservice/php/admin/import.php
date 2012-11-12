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
	<li><a href="index.php">Accueil</a></li>
	<li class="active"><a href="import.php">Import</a></li>
	<li><a href="list.php">Contenu</a></li>
</ul>
</div>
<!--/.well --></div>
<!--/span-->
<div class="span9">
<div class="hero-unit">
<h1>Culturez-Vous !</h1>
<p>Import des tweets</p>
</div>
</div>
<!--/span--> <?php 

// Get Page param
$page = 1;
if(isset($_GET["page"])) {
	$page = $_GET["page"];
}

?>

<div class="span9">

<form class="well form-search" method="get" action="import.php">
	<label>Page number : </label> 
	<input type="text" class="input-medium search-query" value="<?php echo($page); ?>" name="page" /> 
	<input type="submit"	class="btn" value="GET" />
	<?php if(($page - 1) >= 1) { ?>
		<a class="btn" href="import.php?page=<?php echo ($page - 1); ?>"><i class="icon-chevron-left "></i></a>
	<?php } ?>
	<a class="btn" href="import.php?page=<?php echo ($page + 1); ?>"> <i class="icon-chevron-right"></i></a>
</form>

<?php

// Get Page param
$page = 1;
if(isset($_GET["page"])) {
	$page = $_GET["page"];
}
?>
<p>Page <?php echo($page); ?></p>
<table class="table table-striped table-bordered table-condensed">
	<thead>
		<tr>
			<th>Titre</th>
			<th>Contenu</th>
			<th>Ajouté ?</th>
		</tr>
	</thead>
	<tbody>
	<?php

	// Call Twitter API
	$url = "http://api.twitter.com/1/statuses/user_timeline.xml?screen_name=1Jour1Mot&page=$page";
	echo('<p><span class="label label-info">'.$url.'</span></p>');

	$doc = new DOMDocument();
	$doc->load($url);

	$count = 0;

	try
	{
		$bdd = connectDb();
		
		foreach ($doc->getElementsByTagName('status') as $node) {

			foreach($node->childNodes as $enfant)
			{
				$nom = $enfant->nodeName;
				$val = $enfant->nodeValue;

				if($nom == 'text') {

					// Parsing the tweet
					$title1 = strtok($val, ":");
					$content1 = "";
					
					while ($tok = strtok(":")) {
						$content1 .= $tok;
					}

					$found = tweetExists($title1, $content1);

					if(!$found) {

						$count++;

						// Insérer les tweets
						$req = "INSERT INTO temp_tweets (`id`, `title`, `details`, `archived`) VALUES (NULL, '".mysql_real_escape_string($title1)."', '".mysql_real_escape_string($val)."', b'0');";
						queryDb($req, $bdd);
					}
					?>

			<tr>
				<td><?php echo($title1); ?></td>
				<td><?php echo($content1); ?></td>
				<td><?php if(!$found) echo("<span class=\"label label-success\">Oui</span>"); else echo("<span class=\"label\">Non</span>"); ?></td>
			</tr>

			<?php


				}
			}
		}
	
	}
	catch (Exception $e)
	{
		die('Erreur : ' . $e->getMessage());
	}

	?>
	</tbody>
</table>
<p>Total : <span class="badge badge-success"><?php echo($count) ?>
&nbsp;ajouts</span></p>

</div>



</div>
<!--/row-->

<hr>

<footer>
<p>© Valryon 2012</p>
</footer></div>
<!--/.fluid-container-->
</body>
<?php
includes();
?> 
</html>
