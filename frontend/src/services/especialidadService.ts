import type { Especialidad } from '../types'
import { request } from './api'

export const especialidadService = {
  listar: () => request<Especialidad[]>('Especialidades'),
}
