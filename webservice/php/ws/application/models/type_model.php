<?php  if ( ! defined('BASEPATH')) exit('No direct script access allowed');

/**
 * Mod�le du type de l'�l�ment (Mot, contrep�trie, etc.)
 * 
 * @author NPL
 * @date 2012-03
 *
 */
class Type_model extends CI_Model
{
	protected $table = 'type';

	/**
	 * Retourne un type en fonction de son id.
	 * 
	 * @param unknown_type $id
	 */
	public function getType($id)
	{
		
		return $this->db->select('*')
						->from($this->table)
						->where('type_id', (int) $id)
						->get()
						->result();
	}
	
}