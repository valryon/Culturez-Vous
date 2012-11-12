<?php  if ( ! defined('BASEPATH')) exit('No direct script access allowed');

/**
 * Modèle de l'auteur.
 * 
 * @author NPL
 * @date 2012-03
 *
 */
class Author_model extends CI_Model
{
	protected $table = 'authors';

	
	/**
	 * Retourne un auteur en fonction de son id.
	 * 
	 * @param unknown_type $id
	 */
	public function getAuthor($id)
	{
		
		return $this->db->select('*')
						->from($this->table)
						->where('author_id', (int) $id)
						->get()
						->result();
	}
}