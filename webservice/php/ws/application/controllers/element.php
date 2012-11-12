<?php

/**
 * Controller des éléments.
 * 
 * @author NPL
 * @date 2012-03
 *
 */
class Element extends CI_Controller
{
	private $titre_defaut;
	
	/**
	 * Constructeur
	 */
	public function __construct()
	{
		//	Obligatoire
		parent::__construct();
	
		$this->titre_defaut = 'Culturez vous';
		$this->load->database();
		$this->load->helper('xml');
		
		
	}

	/**
	 * Index
	 */
	public function index()
	{
		
		$data = array();
		
		$this->load->model('element_model');
		$data['elements'] = $this->element_model->getElementById();
		
		$this->createElementXML($data['elements']);
	}

	/**
	 * Retourne l'élément de l'id passé en paramètre.
	 * 
	 * @param int $id
	 */
	public function id($id = 0)
	{

		$data = array();
		
		$this->load->model('element_model');
		$data['elements'] = $this->element_model->getElementById($id);
		
		$this->createElementXML($data['elements']);
	}

	/**
	 * Retourne 7 éléments.
	 *
	 * @param int $numPage
	 */
	public function page($numPage = 1)
	{
	
		if($numPage < 1){
			$numPage = 1;
		}
		
		$data = array();
			
		$this->load->model('element_model');
		$data['elements'] = $this->element_model->getElementByPage($numPage);
			
		$this->createElementXML($data['elements']);
		
	}
	
	/**
	 * Retourne un élément pour une date donnée au format aaaa-mm-jj
	 * 
	 * @param unknown_type $date
	 */
	public function date($date = '0000-00-00'){
		
		$data = array();
		
		$this->load->model('element_model');
		$data['elements'] = $this->element_model->getElementByDate($date);
		
		$this->createElementXML($data['elements']);
	}
	
	
	/**
	 * Retourne un élément au hasard
	 */
	public function random(){
		
		
		$data = array();
		
		$this->load->model('element_model');
		$data['elements'] = $this->element_model->getRandomElement();
		
		$this->createElementXML($data['elements']);

	}
	
	/**
	 * Retourne les 20 meilleurs éléments
	 */
	public function bestof(){
		
		$data = array();
		
		$this->load->model('element_model');
		$data['elements'] = $this->element_model->getBestof();
		
		$this->createElementXML($data['elements']);

	}
	
	/**
	 * Génération de l'XML d'un élément.
	 * 
	 * @param unknown_type $elts
	 */
	public function createElementXML($elts){
		
		$dom = xml_dom();
		$elements = xml_add_child($dom, 'elements');
		
		foreach($elts as $elt):
			
			$element = xml_add_child($elements, 'element');
			
		
			//Gestion des types.
			$this->load->model('type_model');			
			$data['type'] = $this->type_model->getType($elt->type_id);
			foreach($data['type'] as $type):
				xml_add_child($element, 'type', $type->type_name);
			endforeach;
			
			//Gestion général de l'élément
			xml_add_child($element, 'id', $elt->element_id);
			xml_add_child($element, 'date', $elt->element_date);
			xml_add_child($element, 'title', stripslashes($elt->element_title));
			//xml_add_child($element, 'preview', $elt->element_preview);
			
			//Gestion des définitions
			$definitions = xml_add_child($element, 'definitions');
			
			$this->load->model('definition_model');
			$data['definition'] = $this->definition_model->getDefinitions($elt->element_id);
			
			foreach($data['definition'] as $def):
				$definition = xml_add_child($definitions, 'definition');
				xml_add_child($definition, 'details', stripslashes($def->definition_detail));
				xml_add_child($definition, 'content', stripslashes($def->definition_content));
			endforeach;
			
			
			//Gestion des contrepétries
			$this->load->model('contrepetrie_model');
			$data['contrepetrie'] = $this->contrepetrie_model->getContrepetrie($elt->element_id);
			
			foreach($data['contrepetrie'] as $ctp):
				xml_add_child($element, 'content', stripslashes($ctp->contrepetrie_content));
				xml_add_child($element, 'solution', stripslashes($ctp->contrepetrie_solution));
			endforeach;
			
			
			//Affichage du compte
			xml_add_child($element, 'voteCount',$elt->element_favoriteCount);
			
			//Gestion des auteurs
			$this->load->model('author_model');
			$data['author'] = $this->author_model->getAuthor($elt->author_id);

			foreach($data['author'] as $author):
				xml_add_child($element, 'author', stripslashes($author->author_name));
				xml_add_child($element, 'authorInfo', stripslashes($author->author_info));
			endforeach;

		endforeach;
		
		xml_print($dom);
		
	}	
	
	//Exemple double param :
	
// 	//	Cette page accepte deux variables $_GET facultatives
// 	public function manger($plat = '', $boisson = '')
// 	{
// 		echo 'Voici votre menu : <br />';
// 		echo $plat . '<br />';
// 		echo $boisson . '<br />';
// 		echo 'Bon appétit !';
// 	}

}
?>