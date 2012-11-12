<?php 
require('functions.php'); 

session_start();

$success = false;
$post = false;

if(isset($_POST["username"])) {

	$post = true;

	$username = $_POST["username"];
	$password = $_POST["password"];
	
	if((strcmp($username,"cvadmin") == 0) 
	&& (strcmp($password,"potpiwisi!") == 0)) {
		$success = true;
		$_SESSION['authentication'] = $success;
	}
}

if($success) {
	header( 'Location: list.php' ) ;
}

?>

<!DOCTYPE html>
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

<div class="container-fluid">

<div class="row-fluid">
	<div class="span9">
		<div class="hero-unit">
			<h1>Culturez-Vous !</h1>
			<p>Authentification</p>
		</div>
	</div>
</div>

<div class="row-fluid">
	<div class="span9">
		<?php
		if($success == false && $post == true) {
		?>
			<div class="alert alert-error">
			  Mauvais identifiant/mot de passe.
			</div>
		<?php
		}
		?>

		<form class="form-horizontal" method="post" action="login.php">

			<div class="control-group">
				<label class="control-label">Identifiant</label>
				<div class="controls">
					<input type="text" class="input-xlarge"	name="username"/>
				</div>
			</div>
			<div class="control-group">
				<label class="control-label">Mot de passe</label>
				<div class="controls">
					<input type="password" class="input-xlarge"	name="password"/>
				</div>
			</div>

			<div class="form-actions">
				<input type="submit" class="btn btn-primary" value="Connexion" />
			</div>

		</form>
	</div>
</div>

</div>
</div>
</body>

</html>