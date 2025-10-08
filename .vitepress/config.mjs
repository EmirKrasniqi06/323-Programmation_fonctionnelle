// Basic config to expose a repo to vitpress with sidebar navigation and search
// It needs either
//     npm install -g vitepress && vitepress dev
//     npx vitepress dev
// Thats it !
import { readdirSync, statSync } from 'fs'
import { join, basename } from 'path'

function buildHierarchy(dir = '.', relativePath = '') {
  const files = []
  const folders = []
  
  try {
    const entries = readdirSync(dir)
    
    for (const entry of entries) {
      // Ignorer
      if (entry === 'node_modules' || entry === '.vitepress' || entry === '.git' || entry.startsWith('.')) continue
      
      const fullPath = join(dir, entry)
      const stat = statSync(fullPath)
      const relPath = relativePath ? `${relativePath}/${entry}` : entry
      
      if (stat.isDirectory()) {
        // Dossier
        const children = buildHierarchy(fullPath, relPath)
        if (children.length > 0) {
          folders.push({
            text: entry,
            collapsed: true,
            items: children
          })
        }
      } else if (entry.endsWith('.md')) {
        // Fichier
        const name = basename(entry, '.md')
        if (name !== 'index') {
          files.push({
            text: name,
            link: `/${relPath.replace('.md', '')}`
          })
        }
      }
    }
  } catch (e) {
    console.error('Error scanning:', dir, e)
  }
  
  // Trier fichiers, puis dossiers, puis concaténer
  files.sort((a, b) => a.text.localeCompare(b.text))
  folders.sort((a, b) => a.text.localeCompare(b.text))
  
  return [...files, ...folders]
}

export default {
  title: 'Programmation Fonctionnelle',
  
  rewrites: {
    'README.md': 'index.md'
  },
  
  themeConfig: {
    sidebar: {
      '/': buildHierarchy()
    },
    
    search: {
      provider: 'local'
    }
  }
}