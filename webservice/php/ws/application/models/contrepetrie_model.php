<?php  if ( ! defined('BASEPATH')) exit('No direct script access allowed');

/**
 * Modèle des définitions de l'élément.
 * 
 * @author NPL
 * @date 2012-03
 *
 */
class Contrepetrie_model extends CI_Model
{
	protected $table = 'contrepetries';
	
	/**
	 * Retourne les définitions en fonction de l'id d'un élément.
	 * 
	 * @param unknown_type $id
	 */
	public function getContrepetrie($id)
	{
		
		return $this->db->select('*')
						->from($this->table)
						->where('element_id', (int) $id)
						->get()
						->result();
	}
	
	
}