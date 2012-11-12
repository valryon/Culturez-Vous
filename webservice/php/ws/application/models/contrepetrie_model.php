<?php  if ( ! defined('BASEPATH')) exit('No direct script access allowed');

/**
 * Mod�le des d�finitions de l'�l�ment.
 * 
 * @author NPL
 * @date 2012-03
 *
 */
class Contrepetrie_model extends CI_Model
{
	protected $table = 'contrepetries';
	
	/**
	 * Retourne les d�finitions en fonction de l'id d'un �l�ment.
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