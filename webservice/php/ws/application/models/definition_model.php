<?php  if ( ! defined('BASEPATH')) exit('No direct script access allowed');

/**
 * Modèle des définitions de l'élément.
 * 
 * @author NPL
 * @date 2012-03
 *
 */
class Definition_model extends CI_Model
{
	protected $table = 'definitions';
	
	/**
	 * Retourne les définitions en fonction de l'id d'un élément.
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