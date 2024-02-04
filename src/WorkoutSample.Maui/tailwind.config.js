/** @type {import('tailwindcss').Config} */
module.exports = {
    content: ['./**/*.{cshtml,razor,html}'],
    corePlugins: {preflight: false},
    theme: {
        extend: {},
    },
    plugins: [require("@tailwindcss/forms")]
}
