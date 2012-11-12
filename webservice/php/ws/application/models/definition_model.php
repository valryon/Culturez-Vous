<?php  if ( ! defined('BASEPATH')) exit('No direct script access allowed');

/**
 * Mod�le des d�finitions de l'�l�ment.
 * 
 * @author NPL
 * @date 2012-03
 *
 */
class Definition_model extends CI_Model
{
	protected $table = 'definitions';
	
	/**
	 * Retourne les d�finitions en fonction de l'id d'un �l�ment.
	 * 
	 * @param unknown_type $id
	 */
	public function getDefinitions($id)
	{
		
		return $this->db->select('*')
						->from($this->table)
						->where('element_id', (int) $id)
						->get()
						->result();
	}
	
	
}