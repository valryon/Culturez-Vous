<?php
/**
 * Auto add includes to libs
 */
function includes() {
?>
	<script type="text/javascript" src="resources/js/jquery-1.7.1.min.js"></script>
	<script type="text/javascript" src="resources/js/jquery-ui-1.8.18.custom.min.js"></script>
	<link type="text/css" href="./resources/css/jquery-ui-1.8.18.custom.css" rel="stylesheet" />	
	<link href="./resources/css/bootstrap.css" rel="stylesheet">
	<link href="./resources/css/bootstrap-responsive.css" rel="stylesheet">
<?php
}

/**
* Manage basic HTTP Authentication
*/
function authentication() {

	session_start();

	$authOk = false;

	if(isset($_SESSION['authentication']))
	{
		$authOk = $_SESSION['authentication'];
	}
	else
	{
		header("location: login.php");
	}

}

/**
 * Establish database connection
 */
function connectDb() {

//	$pdo_options[PDO::ATTR_ERRMODE] = PDO::ERRMODE_EXCEPTION;
//	$pdo_options[PDO::MYSQL_ATTR_INIT_COMMAND] = "SET NAMES 'utf8'";
	
	// Local
//	$bdd = new PDO('mysql:host=localhost;dbname=thegreatzcvs', 'root', '', $pdo_options);
	
	// Online OVH TGPA
//	$bdd = new PDO('mysql:host=mysql51-55.perso;dbname=thegreatzcvs', 'thegreatzcvs', 'Vxzh0FZy', $pdo_options);
	
	
	$host = "mysql51-55.perso";
	$user = "thegreatzcvs";
	$password = "Vxzh0FZy";
	$dbname = "thegreatzcvs"; 
	
	/*
	$host = "localhost";
	$user = "root";
	$password = "";
	$dbname = "thegreatzcvs";
	*/
	
	$bdd = mysql_connect($host, $user, $password);
	mysql_select_db($dbname); 
	mysql_query("SET NAMES UTF8");
	
	if (!$bdd) {
		die("Failed connecting to db: ".mysql_error());
	}
	


	return $bdd;
}

function queryDb($sql, $db)
{
	return mysql_query($sql, $db);
}

function tweetExists($title, $details) {
	try
	{
		$bdd = connectDb();

		$req = "SELECT * FROM temp_tweets WHERE title = '".mysql_real_escape_string($title)."' AND details LIKE '%".mysql_real_escape_string($details)."%';";
		$response = queryDb($req, $bdd);
		
		while ($donnees = mysql_fetch_array($response))
		{
			return true;
		}
		
		return false;
	}
	catch (Exception $e)
	{
		die('Erreur : ' . $e->getMessage());
	}
}

function getTweet($id) {
	try
	{
		$bdd = connectDb();

		// Insérer les tweets
		$req = "SELECT * FROM temp_tweets WHERE id = '".$id."';";
		$response = queryDb($req, $bdd);
		
		while ($donnees = mysql_fetch_array($response))
		{
			return $donnees["details"];
		}
	}
	catch (Exception $e)
	{
		die('Erreur : ' . $e->getMessage());
	}
}

function getTweetsForEdit() {
	try
	{
		$bdd = connectDb();

		// Insérer les tweets
		$req = "SELECT * FROM temp_tweets WHERE archived=b'0';";
				
		return queryDb($req, $bdd);
	}
	catch (Exception $e)
	{
		die('Erreur : ' . $e->getMessage());
	}
}

function archiveTweet($id) {
	try
	{
		$bdd = connectDb();

		// Insérer les tweets
		$req = "UPDATE temp_tweets SET `archived` = 1 WHERE id = '".$id."';";
		queryDb($req, $bdd);
	}
	catch (Exception $e)
	{
		die('Erreur : ' . $e->getMessage());
	}
}

/**
 * Retrieve all generic elements
 * @return database selector
 */
function getElements() {
	try
	{
		$bdd = connectDb();

		// Récupérer données
		$req = "SELECT e.element_id, t.type_name, e.element_title, e.element_date, e.element_favoriteCount, a.author_name FROM elements e, types t, authors a WHERE e.type_id = t.type_id AND   e.author_id = a.author_id ORDER BY `element_date` ASC ";
		return queryDb($req, $bdd);
	}
	catch (Exception $e)
	{
		die('Erreur : ' . $e->getMessage());
	}
}

/**
 * Get author id from its name
 * @param $author_name
 */
function getAuthorId($author_name) {
	try
	{
		$bdd = connectDb();

		// Get author
		$req = "SELECT author_id FROM authors WHERE author_name = '$author_name';";
		$response = queryDb($req, $bdd);

		while ($donnees = mysql_fetch_array($response))
		{
			$author_id = $donnees["author_id"];
			break;
		}

		return $author_id;
	}
	catch (Exception $e)
	{
		die('Erreur : ' . $e->getMessage());
	}
}

/**
 * Get all authors
 */
function getAuthors() {

	try {
		$bdd = connectDb();

		// Récupérer données
		$req = "SELECT * FROM authors WHERE author_id != 1";

		return queryDb($req, $bdd);
	}
	catch (Exception $e)
	{
		die('Erreur : ' . $e->getMessage());
	}
}

/**
 * Retrieve word from id
 * @param $id
 */
function getWord($id) {
	try
	{
		$element = array();

		$bdd = connectDb();

		$req = "SELECT * FROM elements WHERE element_id = '$id';";
		$response = queryDb($req, $bdd);

		while ($donnees = mysql_fetch_array($response))
		{
			$element["id"] = $id;
			$element["date"] = $donnees["element_date"];
			$element["title"] = $donnees["element_title"];
			$element["voteCount"] = $donnees["element_favoriteCount"];
			$element["author_id"] = $donnees["author_id"];
		}

		$req = "SELECT * FROM definitions WHERE element_id = '$id' ORDER BY element_id;";
		$response = queryDb($req, $bdd);

		$element["details1"] = "";
		$element["def1"] = "";
		$element["details2"] = "";
		$element["def2"] = "";
		$element["details3"] = "";
		$element["def3"] ="";

		$i = 0;
		while ($donnees = mysql_fetch_array($response))
		{
			if($i == 0) {
				$element["details1"] = $donnees["definition_detail"];
				$element["def1"] = $donnees["definition_content"];
			} else if($i == 1) {
				$element["details2"] = $donnees["definition_detail"];
				$element["def2"] = $donnees["definition_content"];
			} else {
				$element["details3"] = $donnees["definition_detail"];
				$element["def3"] = $donnees["definition_content"];
			}

			$i++;
			if($i > 2) break;
		}

		return $element;
	}
	catch (Exception $e)
	{
		die('Erreur : ' . $e->getMessage());
	}
}

/**
 * Retrieve contrepeterie from id
 * @param $id
 */
function getCTP($id) {
	try
	{
		$element = array();

		$bdd = connectDb();

		$req = "SELECT * FROM elements WHERE element_id = '$id';";
		$response = queryDb($req, $bdd);

		while ($donnees = mysql_fetch_array($response))
		{
			$element["id"] = $id;
			$element["date"] = $donnees["element_date"];
			$element["title"] = $donnees["element_title"];
			$element["voteCount"] = $donnees["element_favoriteCount"];
			$element["author_id"] = $donnees["author_id"];
		}

		$req = "SELECT * FROM contrepetries WHERE element_id = '$id';";
		$response = queryDb($req, $bdd);

		$element["content"] = "";
		$element["solution"] = "";
		
		while ($donnees = mysql_fetch_array($response))
		{
			$element["content"] = $donnees["contrepetrie_content"];
			$element["solution"] = $donnees["contrepetrie_solution"];
		}

		
		return $element;
	}
	catch (Exception $e)
	{
		die('Erreur : ' . $e->getMessage());
	}
}

/**
 * Update a word in database
 * @param $id
 * @param $title
 * @param $date
 * @param $votecount
 * @param $author
 * @param $details1
 * @param $def1
 * @param $details2
 * @param $def2
 * @param $details3
 * @param $def3
 */
function editWord($id, $title, $date, $votecount, $author, $details1, $def1, $details2, $def2, $details3, $def3) {

	$bdd = connectDb();

	$author_id = getAuthorId($author);

	if($title == '') {
		die("Titre vide !");
	}
	
	// Delete all definitions !
	$req = "DELETE FROM definitions WHERE element_id='$id'; ";

	queryDb($req, $bdd);

	// Update element
	$req = "UPDATE `elements` ".
		"SET `element_date`='$date',`element_title`='".mysql_real_escape_string($title)."',`element_favoriteCount`='".$votecount."',`author_id`='$author_id' WHERE `elements`.element_id='$id';";

	queryDb($req, $bdd);

	// Insert definitions
	if($details1 != '' && $def1 != '') {
		$req = "INSERT INTO `definitions`(`element_id`, `definition_detail`, `definition_content`) ".
			"VALUES ('$id','".mysql_real_escape_string($details1)."','".mysql_real_escape_string($def1)."');";
		$response = queryDb($req, $bdd);
	}

	if($details2 != '' && $def2 != '') {
		$req = "INSERT INTO `definitions`(`element_id`, `definition_detail`, `definition_content`) ".
				"VALUES ('$id','".mysql_real_escape_string($details2)."','".mysql_real_escape_string($def2)."');";
		$response = queryDb($req, $bdd);
	}

	if($details3 != '' && $def3 != '') {
		$req = "INSERT INTO `definitions`(`element_id`, `definition_detail`, `definition_content`) ".
				"VALUES ('$id','".mysql_real_escape_string($details3)."','".mysql_real_escape_string($def3)."');";
		$response = queryDb($req, $bdd);
	}
}

/**
 * Create a new word in database
 * @param unknown_type $title
 * @param unknown_type $date
 * @param unknown_type $votecount
 * @param unknown_type $author
 * @param unknown_type $details1
 * @param unknown_type $def1
 * @param unknown_type $details2
 * @param unknown_type $def2
 * @param unknown_type $details3
 * @param unknown_type $def3
 */
function createWord($title, $date, $votecount, $author, $details1, $def1, $details2, $def2, $details3, $def3) {

	try
	{
		$bdd = connectDb();

		$author_id = getAuthorId($author);

		if($title == '') {
			die("Titre vide !");
		}
				
		// Element
		$req = "INSERT INTO `elements`(`element_id`, `type_id`, `element_date`, `element_title`, `element_favoriteCount`, `author_id`) "
		."VALUES ('NULL','1','".mysql_real_escape_string($date)."','".mysql_real_escape_string($title)."','".mysql_real_escape_string($votecount)."','$author_id');";
		
		queryDb($req, $bdd);

		// Get id
		$id = mysql_insert_id();

		// Insert definitions
		if($details1 != '' && $def1 != '') {
			$req = "INSERT INTO `definitions`(`element_id`, `definition_detail`, `definition_content`) ".
				"VALUES ('$id','".mysql_real_escape_string($details1)."','".mysql_real_escape_string($def1)."');";
			$response = queryDb($req, $bdd);
		}

		if($details2 != '' && $def2 != '') {
			$req = "INSERT INTO `definitions`(`element_id`, `definition_detail`, `definition_content`) ".
					"VALUES ('$id','".mysql_real_escape_string($details2)."','".mysql_real_escape_string($def2)."');";
			$response = queryDb($req, $bdd);
		}

		if($details3 != '' && $def3 != '') {
			$req = "INSERT INTO `definitions`(`element_id`, `definition_detail`, `definition_content`) ".
					"VALUES ('$id','".mysql_real_escape_string($details3)."','".mysql_real_escape_string($def3)."');";
			$response = queryDb($req, $bdd);
		}


	}
	catch (Exception $e)
	{
		die('Erreur ajout mot : ' . $e->getMessage());
	}

}

/**
 * 
 * Delete element 
 * @param $id
 */
function delete($id) {
		try
	{
		$bdd = connectDb();

		$req = "DELETE FROM `elements` WHERE `element_id`='$id';";
		queryDb($req, $bdd);

		$req = "DELETE FROM `definitions` WHERE `element_id`='$id';";
		queryDb($req, $bdd);

		$req = "DELETE FROM `contrepetries` WHERE `element_id`='$id';";
		queryDb($req, $bdd);
	}
	catch (Exception $e)
	{
		die('Erreur : ' . $e->getMessage());
	}
	
}

/**
 * Edit an existing contrepeterie
 * @param $id
 * @param $title
 * @param $date
 * @param $votecount
 * @param $author
 * @param $content
 * @param $solution
 * @param $preview
 */
function editContrepeterie($id, $title, $date, $votecount, $author,$content,$solution) {
	try
	{
		$bdd = connectDb();

		$author_id = getAuthorId($author);

		if($title == '') {
			die("Titre vide !");
		}
		
		$req =  "DELETE FROM  `contrepetries` WHERE  `contrepetries`.`element_id` = '$id';";
		queryDb($req, $bdd);
		
		// Update element
		$req = "UPDATE `elements` ".
		"SET `element_date`='$date',`element_title`='".mysql_real_escape_string($title)."',`element_favoriteCount`='".$votecount."',`author_id`='$author_id' WHERE `elements`.element_id='$id';";

		queryDb($req, $bdd);

		// Update content
		$req = "INSERT INTO `contrepetries`(`element_id`, `contrepetrie_content`, `contrepetrie_solution`) ".
			"VALUES ('$id','".mysql_real_escape_string($content)."','".mysql_real_escape_string($solution)."');";
		
		queryDb($req, $bdd);

	}
	catch (Exception $e)
	{
		die('Erreur ajout contrepétrie : ' . $e->getMessage());
	}
}

/**
 * Create a new contrepeterie
 * @param $title
 * @param $date
 * @param $votecount
 * @param $author
 * @param $content
 * @param $solution
 * @param $preview
 */
function createContrepeterie($title, $date, $votecount, $author,$content,$solution){
	try
	{
		$bdd = connectDb();

		$author_id = getAuthorId($author);

		if($title == '') {
			die("Titre vide !");
		}
		
		// Element
		$req = "INSERT INTO `elements`(`element_id`, `type_id`, `element_date`, `element_title`, `element_favoriteCount`, `author_id`) "
		."VALUES ('NULL','2','".mysql_real_escape_string($date)."','".mysql_real_escape_string($title)."','".mysql_real_escape_string($votecount)."','$author_id');";

		queryDb($req, $bdd);

		// Get id
		$id = mysql_insert_id();

		// Insert content
		$req = "INSERT INTO `contrepetries`(`element_id`, `contrepetrie_content`, `contrepetrie_solution`) ".
			"VALUES ('$id','".mysql_real_escape_string($content)."','".mysql_real_escape_string($solution)."');";
		
		$response = queryDb($req, $bdd);

	}
	catch (Exception $e)
	{
		die('Erreur ajout contrepétrie : ' . $e->getMessage());
	}
}

function getNextWordDate() {
	$bdd = connectDb();
	$req = "SELECT DATE_ADD(MAX(element_date),INTERVAL 1 DAY) as nextWordDate from elements where type_id=1";
	
	$response = queryDb($req, $bdd);
	
	while ($donnees = mysql_fetch_array($response))
	{
		return $donnees["nextWordDate"];
	}
	
	return null;
}

function getNextCTPDate() {
	$bdd = connectDb();
	$req = "SELECT DATE_ADD(MAX(element_date),INTERVAL 7 DAY) as nextCtpDate from elements where type_id=2;";
	
	$response = queryDb($req, $bdd);
	
		while ($donnees = mysql_fetch_array($response))
	{
		return $donnees["nextCtpDate"];
	}
	
	return null;
}

?>