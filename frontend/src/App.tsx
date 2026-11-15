import { NavLink, Route, Routes } from 'react-router-dom'
import Panel from './pages/Panel'
import Pacientes from './pages/Pacientes'
import Medicos from './pages/Medicos'
import Citas from './pages/Citas'

const enlaces = [
  { ruta: '/', texto: 'Panel' },
  { ruta: '/pacientes', texto: 'Pacientes' },
  { ruta: '/medicos', texto: 'Médicos' },
  { ruta: '/citas', texto: 'Citas' },
]

export default function App() {
  return (
    <div className="app">
      <aside className="menu">
        <h1 className="marca">HealthSync</h1>
        <p className="eslogan">
          <span className="tono1">Tu salud </span>
          <span className="tono2">able</span>
        </p>
        <nav>
          {enlaces.map((e) => (
            <NavLink
              key={e.ruta}
              to={e.ruta}
              end={e.ruta === '/'}
              className={({ isActive }) => (isActive ? 'enlace activo' : 'enlace')}
            >
              {e.texto}
            </NavLink>
          ))}
        </nav>
      </aside>
      <main className="contenido">
        <Routes>
          <Route path="/" element={<Panel />} />
          <Route path="/pacientes" element={<Pacientes />} />
          <Route path="/medicos" element={<Medicos />} />
          <Route path="/citas" element={<Citas />} />
        </Routes>
      </main>
    </div>
  )
}
