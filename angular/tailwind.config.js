/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors:{
        'science-blue': {
            '50': '#f1f7fe',
            '100': '#e1eefd',
            '200': '#bddbfa',
            '300': '#82bef7',
            '400': '#409ef0',
            '500': '#1782e0',
            '600': '#0a66c2',
            '700': '#09509b',
            '800': '#0c4480',
            '900': '#103b6a',
            '950': '#0b2546',
        },
      }
    },
  },
  plugins: [],
}

