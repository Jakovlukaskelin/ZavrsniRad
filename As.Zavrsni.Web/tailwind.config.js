/** @type {import('tailwindcss').Config} */
module.exports = {
    mode: "jit",
    content: ["./**/*.{razor,html,cshtml,js}"],
    theme: {
        extend: {
            colors: {
                background: "#edeff2",
                green: {
                    50: "#e8f8e8",
                    100: "#d0f0d0",
                    200: "#a1e1a1",
                    300: "#72d272",
                    400: "#43c343",
                    500: "#14b414", //default
                    600: "#109010",
                    700: "#0c6c0c",
                    800: "#084808",
                    900: "#042404",
                },
                'footer-color': '#252525',
                'mischka': {
                    '50': '#f5f6f8',
                    '100': '#edeff2',
                    '200': '#dee1e7',
                    '300': '#d2d6de',
                    '400': '#b3b8c6',
                    '500': '#9ea3b6',
                    '600': '#888aa3',
                    '700': '#75778d',
                    '800': '#606273',
                    '900': '#51535e',
                    '950': '#2f2f37',
                },
                'astral': {
                    '50': '#f4f8fb',
                    '100': '#e7f1f7',
                    '200': '#cae0ed',
                    '300': '#9bc6de',
                    '400': '#65a8cb',
                    '500': '#428eb5',
                    '600': '#367fa9',
                    '700': '#285c7c',
                    '800': '#254e67',
                    '900': '#234357',
                    '950': '#172b3a',
                },
                'info': {
                    '50': '#effbfc',
                    '100': '#d8f4f5',
                    '200': '#b5e7ec',
                    '300': '#82d6de',
                    '400': '#39b2c1', //default is 400
                    '500': '#2c9eae',
                    '600': '#278093',
                    '700': '#266878',
                    '800': '#275663',
                    '900': '#244955',
                    '950': '#132f39',
                },
                'warning': {
                    '50': '#fff8ed',
                    '100': '#ffefd4',
                    '200': '#ffdaa9',
                    '300': '#ffc073', //default is 300
                    '400': '#fe9a39',
                    '500': '#fc7b13',
                    '600': '#ed6009',
                    '700': '#c54709',
                    '800': '#9c3810',
                    '900': '#7e3010',
                    '950': '#441606',
                },
                'error': {
                    '50': '#fdf4f3',
                    '100': '#fce6e4',
                    '200': '#fad2ce',
                    '300': '#f39d94', //default is 300
                    '400': '#ee867b',
                    '500': '#e25f51',
                    '600': '#cf4233',
                    '700': '#ad3528',
                    '800': '#902e24',
                    '900': '#782c24',
                    '950': '#41130e',
                },
            },
        },
        fontFamily: {
            sans: ['"Open Sans"', 'sans-serif'],
            'condensed': ['"Open Sans Condensed"', 'sans-serif'],
            'semi-condensed': ['"Open Sans SemiCondensed"', 'sans-serif'],
        },
    },
    corePlugins: {
        container: false
    },
    plugins: [
        function ({ addComponents }) {
            addComponents({
                '.container': {
                    maxWidth: '100%',
                    '@screen sm': {
                        maxWidth: '540px',
                    },
                    '@screen md': {
                        maxWidth: '720px',
                    },
                    '@screen lg': {
                        maxWidth: '960px',
                    },
                    '@screen xl': {
                        maxWidth: '1140px',
                    },
                }
            })
        }
    ]
};
