<?php  if ( ! defined('BASEPATH')) exit('No direct script access allowed');

/**
 * Mod�le des �l�ments.
 * 
 * @author NPL
 * @date 2012-03
 *
 */
class Element_model extends CI_Model
{
	protected $table = 'elements';

	/**
	 * Retourne un �l�ment dont l'id est pass� en param�tre.
	 * 
	 * @param unknown_type $id
	 */
	public function getElementById($id = 0)
	{
		
		if($id==0){

			$d = date('Y-m-d')  . " 23:59:59";
			
			
			return $this->db->select('*')
						->from($this->table)
						->where('element_date <=', $d)
						->order_by('element_date', 'desc')
						->limit(7)
						->get()
						->result();
		}
		else{
			
			$d = date('Y-m-d')  . " 23:59:59";
			
			return $this->db->select('*')
						->from($this->table)
						->where('element_id', (int) $id)
						->where('element_date <=', $d)
						->get()
						->result();
		}
		
	
	}
	
	
	/**
	 * Retourne une page de 7 �l�ments.
	 *
	 * @param int $numPage
	 */
	public function getElementByPage($numPage = 1)
	{
		$d = date('Y-m-d')  . " 23:59:59";
		
		return $this->db->select('*')
		->from($this->table)
		->where('element_date <=', $d)
		->order_by('element_date', 'desc')
		->limit(7, 7*($numPage-1))
		->get()
		->result();
	}
	
	/**
	 * Retourne un �l�ment pour la date pass�e en param�tre (au format aaaa-mm-jj).
	 * 
	 * @param unknown_type $date
	 */
	public function getElementByDate($date = '0000-00-00')
	{
		
		$query = $this->db->query("SELECT * FROM " . $this->table . " WHERE  element_date >= '" . $date . " 00:00:00' AND element_date <= '" . $date . " 23:59:59'" );
		
		return $query->result();
	}
	
	
	/**
	 * Retourne un �l�ment au hasard.
	 * 
	 */
	public function getRandomElement(){
		
		
		$d = date('Y-m-d')  . " 23:59:59";
		
		$query = $this->db->query("SELECT * FROM " . $this->table . " WHERE element_date <= '" . $d  . "' ORDER BY RAND() LIMIT 1" );
		
		return $query->result();

	}
	
	/**
	 * Retourne les 20 premiers �l�ments
	 * 
	 */
	public function getBestof(){
		$d = date('Y-m-d')  . " 23:59:59";
		
		$query = $this->db->query("SELECT * FROM " . $this->table . " WHERE element_date <= '" . $d  . "' ORDER BY  `element_favoriteCount` DESC LIMIT 20" );
		
		return $query->result();

	}
	
	
}